using System;
using System.Collections.Generic;
using System.Threading;

namespace UFramework
{
    public class Application : Container, IApplication
    {
        private const string version = "1.0.0";
        private readonly IList<IServiceProvider> loadedProviders;
        private readonly int mainThreadId;
        private readonly IDictionary<Type, string> dispatchMapping;
        private bool bootstrapped;
        private bool inited;
        private bool registering;
        private long incrementId;
        private DebugLevel debugLevel;
        private IEventDispatcher dispatcher;

        public Application()
        {
            loadedProviders = new List<IServiceProvider>();

            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            RegisterBaseBindings();

            dispatchMapping = new Dictionary<Type, string>()
            {
                { typeof(AfterBootEventArgs), ApplicationEvents.OnAfterBoot },
                { typeof(AfterInitEventArgs), ApplicationEvents.OnAfterInit },
                { typeof(AfterTerminateEventArgs), ApplicationEvents.OnAfterTerminate },
                { typeof(BeforeBootEventArgs), ApplicationEvents.OnBeforeBoot },
                { typeof(BeforeInitEventArgs), ApplicationEvents.OnBeforeInit },
                { typeof(BeforeTerminateEventArgs), ApplicationEvents.OnBeforeTerminate },
                { typeof(BootingEventArgs), ApplicationEvents.OnBooting },
                { typeof(InitProviderEventArgs), ApplicationEvents.OnInitProvider },
                { typeof(RegisterProviderEventArgs), ApplicationEvents.OnRegisterProvider },
                { typeof(StartCompletedEventArgs), ApplicationEvents.OnStartCompleted },
            };

            // 我们使用闭包来保存当前上下文状态。
            // 不要更改为：OnFindType（Type.GetType）。
            // 这会导致活动程序集不是预期的作用域
            OnFindType(finder => { return Type.GetType(finder); });

            DebugLevel = DebugLevel.Production;
            Process = StartProcess.Construct;
        }

        /// <summary>
        /// 获取框架应用程序的版本
        /// </summary>
        public static string Version => version;

        /// <summary>
        /// 获取当前应用程序所在的进程。
        /// </summary>
        public StartProcess Process { get; private set; }

        public bool IsMainThread => mainThreadId == Thread.CurrentThread.ManagedThreadId;

        public DebugLevel DebugLevel
        {
            get => debugLevel;
            set
            {
                debugLevel = value;
                this.Instance<DebugLevel>(debugLevel);
            }
        }

        public static Application New(bool global = true)
        {
            var application = new Application();
            if (global)
            {
                App.That = application;
            }

            return application;
        }

        /// <summary>
        /// 设置事件派发器
        /// </summary>
        public void SetDispatcher(IEventDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.Instance<IEventDispatcher>(dispatcher);
        }

        public IEventDispatcher GetDispatcher()
        {
            return dispatcher;
        }

        public virtual void Terminate()
        {
            Process = StartProcess.Terminate;
            Raise(new BeforeTerminateEventArgs(this));
            Process = StartProcess.Terminating;
            Flush();
            if (App.That == this)
            {
                App.That = null;
            }

            Process = StartProcess.Terminated;
            Raise(new AfterTerminateEventArgs(this));
        }

        /// <summary>
        /// 启动引导程序 
        /// </summary>
        public virtual void Bootstrap(params IBootstrap[] bootstraps)
        {
            Guard.Requires<ArgumentNullException>(bootstraps != null);

            if (bootstrapped || Process != StartProcess.Construct)
            {
                throw new LogicException($"Cannot repeatedly trigger the {nameof(Bootstrap)}()");
            }

            Process = StartProcess.Bootstrap;
            bootstraps = Raise(new BeforeBootEventArgs(bootstraps, this))
                .GetBootstraps();
            Process = StartProcess.Bootstrapping;

            var existed = new HashSet<IBootstrap>();

            foreach (var bootstrap in bootstraps)
            {
                if (bootstrap == null)
                {
                    continue;
                }

                if (existed.Contains(bootstrap))
                {
                    throw new LogicException($"引导程序已经存在 : {bootstrap}");
                }

                existed.Add(bootstrap);

                var skipped = Raise(new BootingEventArgs(bootstrap, this))
                    .IsSkip;
                if (!skipped)
                {
                    bootstrap.Bootstrap();
                }
            }

            Process = StartProcess.Bootstraped;
            bootstrapped = true;
            Raise(new AfterBootEventArgs(this));
        }

        /// <summary>
        /// 初始化所有注册的服务提供者 
        /// </summary>
        public virtual void Init()
        {
            if (!bootstrapped)
            {
                throw new LogicException($"You must call {nameof(Bootstrap)}() first.");
            }

            if (inited || Process != StartProcess.Bootstraped)
            {
                throw new LogicException($"Cannot repeatedly trigger the {nameof(Init)}()");
            }

            Process = StartProcess.Init;
            Raise(new BeforeInitEventArgs(this));
            Process = StartProcess.Initing;

            foreach (var provider in loadedProviders)
            {
                InitProvider(provider);
            }

            inited = true;
            Process = StartProcess.Inited;
            Raise(new AfterInitEventArgs(this));

            Process = StartProcess.Running;
            Raise(new StartCompletedEventArgs(this));
        }

        public virtual void Register(IServiceProvider provider, bool force = false)
        {
            Guard.Requires<ArgumentNullException>(provider != null,
                $"Parameter \"{nameof(provider)}\" can not be null.");

            if (IsRegistered(provider))
            {
                if (!force)
                {
                    throw new LogicException($"Provider [{provider.GetType()}] is already register.");
                }

                loadedProviders.Remove(provider);
            }

            if (Process == StartProcess.Initing)
            {
                throw new LogicException($"Unable to add service provider during {nameof(StartProcess.Initing)}");
            }

            if (Process > StartProcess.Running)
            {
                throw new LogicException($"Unable to {nameof(Terminate)} in-process registration service provider");
            }

            if (provider is ServiceProvider baseProvider)
            {
                baseProvider.SetApplication(this);
            }

            var skipped = Raise(new RegisterProviderEventArgs(provider, this))
                .IsSkip;
            if (skipped)
            {
                return;
            }

            try
            {
                registering = true;
                provider.Register();
            }
            finally
            {
                registering = false;
            }

            loadedProviders.Add(provider);

            if (inited)
            {
                InitProvider(provider);
            }
        }

        protected override void GuardConstruct(string method)
        {
            if (registering)
            {
                throw new LogicException(
                    $"It is not allowed to make services or dependency injection in the {nameof(Register)} process, method:{method}");
            }

            base.GuardConstruct(method);
        }

        public bool IsRegistered(IServiceProvider provider)
        {
            Guard.Requires<ArgumentNullException>(provider != null);
            return loadedProviders.Contains(provider);
        }

        public long GetRuntimeId()
        {
            return Interlocked.Increment(ref incrementId);
        }

        /// <summary>
        /// 初始化指定的服务提供者 
        /// </summary>
        protected virtual void InitProvider(IServiceProvider provider)
        {
            Raise(new InitProviderEventArgs(provider, this));
            provider.Init();
        }

        private void RegisterBaseBindings()
        {
            this.Singleton<IApplication>(() => this);
            SetDispatcher(new EventDispatcher());
        }

        private T Raise<T>(T args) where T : EventParam
        {
            if (!dispatchMapping.TryGetValue(args.GetType(), out string eventName))
            {
                throw new AssertException($"Assertion error: 未定义的事件 {args}");
            }

            if (dispatcher == null)
            {
                return args;
            }

            dispatcher.Raise(eventName, this, args);
            return args;
        }
    }
}