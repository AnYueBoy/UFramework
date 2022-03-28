using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UFramework.Exception;
using UFramework.Util;
using SException = System.Exception;

namespace UFramework.Container
{
    public class Container : IContainer
    {
        private static readonly char[] ServiceBanChars = {'@', ':', '$'};

        private readonly Dictionary<string, BindData> bindings;

        private readonly Dictionary<string, object> instances;

        private readonly Dictionary<object, string> instancesReverse;

        private readonly List<Action<IBindData, object>> resolving;

        private readonly List<Action<IBindData, object>> afterResloving;

        private readonly List<Action<IBindData, object>> release;

        private readonly SortSet<Func<string, Type>, int> findType;

        private readonly Dictionary<string, Type> findTypeCache;

        private readonly HashSet<string> resolved;

        private readonly Dictionary<string, List<Action<object>>> rebound;

        private bool flushing;

        protected Stack<string> BuildStack { get; }

        protected Stack<object[]> UserParamsStack { get; }

        public Container(int prime = 64)
        {
            prime = Math.Max(8, prime);
            instances = new Dictionary<string, object>(prime * 4);
            instancesReverse = new Dictionary<object, string>(prime * 4);
            bindings = new Dictionary<string, BindData>(prime * 4);
            resolving = new List<Action<IBindData, object>>((int) (prime * 0.25));
            afterResloving = new List<Action<IBindData, object>>((int) (prime * 0.25));
            release = new List<Action<IBindData, object>>((int) (prime * 0.25));
            resolved = new HashSet<string>();
            findType = new SortSet<Func<string, Type>, int>();
            findTypeCache = new Dictionary<string, Type>(prime * 4);
            rebound = new Dictionary<string, List<Action<object>>>(prime);

            BuildStack = new Stack<string>(32);
            UserParamsStack = new Stack<object[]>(32);
            flushing = false;
        }

        public IBindData GetBind(string service)
        {
            if (string.IsNullOrEmpty(service))
            {
                return null;
            }

            return bindings.TryGetValue(service, out BindData bindData) ? bindData : null;
        }

        public bool HasBind(string service)
        {
            return GetBind(service) != null;
        }

        public bool HasInstance(string service)
        {
            Guard.ParameterNotNull(service, nameof(service));

            return instances.ContainsKey(service);
        }

        public bool IsResolved(string service)
        {
            Guard.ParameterNotNull(service, nameof(service));
            return resolved.Contains(service) || instances.ContainsKey(service);
        }

        public bool CanMake(string service)
        {
            Guard.ParameterNotNull(service, nameof(service));

            if (HasBind(service) || HasInstance(service))
            {
                return true;
            }

            Type type = SpeculatedServiceType(service);
            return !IsBasicType(type) && !IsUnableType(type);
        }

        public bool IsStatic(string service)
        {
            IBindData bind = GetBind(service);
            return bind != null && bind.IsStatic;
        }

        public IBindData Bind(string service, Type concrete, bool isStatic)
        {
            Guard.Requires<ArgumentNullException>(concrete != null, $"Parameter {nameof(concrete)} can not be null.");

            if (IsUnableType(concrete))
            {
                throw new LogicException(
                    $"Type \"{concrete}\" can not bind. please check if there is a list of types that cannot be built.");
            }

            service = FormatService(service);
            return Bind(service, WrapperTypeBuilder(service, concrete), isStatic);
        }

        protected Func<IContainer, object[], object> WrapperTypeBuilder(string service, Type concrete)
        {
            return (container, userParams) =>
                ((Container) container).CreateInstance(GetBindFillable(service), concrete, userParams);
        }

        protected BindData GetBindFillable(string service)
        {
            return service != null && bindings.TryGetValue(service, out BindData bindData)
                ? bindData
                : MakeEmptyBindData(service);
        }

        protected BindData MakeEmptyBindData(string service)
        {
            return new BindData(this, service, null, false);
        }

        public IBindData Bind(string service, Func<IContainer, object[], object> concrete, bool isStatic)
        {
            Guard.ParameterNotNull(service, nameof(service));
            Guard.ParameterNotNull(concrete, nameof(concrete));
            GuardServiceName(service);
            GuardFlushing();

            service = FormatService(service);
            if (bindings.ContainsKey(service))
            {
                throw new LogicException($"Bind [{service}] already exists.");
            }

            if (instances.ContainsKey(service))
            {
                throw new LogicException($"Instances [{service}] is already exists.");
            }

            // concrete 根据类型用反射创建实例
            BindData bindData = new BindData(this, service, concrete, isStatic);
            bindings.Add(service, bindData);

            if (!IsResolved(service))
            {
                return bindData;
            }

            if (isStatic)
            {
                // If it is "static" then solve this service directly
                // The process of staticing the service triggers TriggerOnRebound
                Make(service);
            }
            else
            {
                TriggerOnRebound(service);
            }

            return bindData;
        }

        public object Make(string service, params object[] userParams)
        {
            return Resolve(service, userParams);
        }

        protected object Resolve(string service, params object[] userParams)
        {
            Guard.ParameterNotNull(service, nameof(service));

            // 如果单例池中有对应服务则直接返回否则进入构建过程
            if (instances.TryGetValue(service, out object instance))
            {
                return instance;
            }

            if (BuildStack.Contains(service))
            {
                throw MakeCircularDependencyException(service);
            }

            BuildStack.Push(service);
            UserParamsStack.Push(userParams);
            try
            {
                BindData bindData = GetBindFillable(service);

                // We will start building a service instance.
                // For the built service we will try to do dependency injection.

                // 构建实例
                instance = Build(bindData, userParams);

                instance = bindData.IsStatic
                    ? Instance(bindData.Service, instance)
                    : TriggerOnResolving(bindData, instance);

                resolved.Add(bindData.Service);
                return instance;
            }
            finally
            {
                UserParamsStack.Pop();
                BuildStack.Pop();
            }
        }

        protected virtual object Build(BindData makeServiceBindData, object[] userParams)
        {
            object instance = makeServiceBindData.Concrete != null
                ? makeServiceBindData.Concrete(this, userParams)
                : CreateInstance(makeServiceBindData, SpeculatedServiceType(makeServiceBindData.Service), userParams);

            return instance;
        }

        protected virtual object CreateInstance(BindData makeServiceBindData, Type makeServiceType, object[] userParams)
        {
            if (IsUnableType(makeServiceType))
            {
                return null;
            }

            try
            {
                return CreateInstance(makeServiceType, userParams);
            }
            catch (SException ex)
            {
                throw MakeBuildFailedException(makeServiceBindData.Service, makeServiceType, ex);
            }
        }

        protected virtual object CreateInstance(Type makeService, object[] userParams)
        {
            if (userParams == null || userParams.Length <= 0)
            {
                return Activator.CreateInstance(makeService);
            }

            return Activator.CreateInstance(makeService, userParams);
        }

        public object Instance(string service, object instance)
        {
            Guard.ParameterNotNull(service, nameof(service));
            GuardFlushing();
            GuardServiceName(service);

            IBindData bindData = GetBind(service);
            if (bindData != null)
            {
                if (!bindData.IsStatic)
                {
                    throw new LogicException($"Service [{service}] is not Singleton(Static) Bind.");
                }
            }
            else
            {
                bindData = MakeEmptyBindData(service);
            }

            instance = TriggerOnResolving((BindData) bindData, instance);

            if (instance != null &&
                instancesReverse.TryGetValue(instance, out string realService) &&
                realService != service)
            {
                throw new LogicException($"The instance has been registered as a singleton in {realService}.");
            }

            bool isResolved = IsResolved(service);
            Release(service);

            instances.Add(service, instance);

            if (instance != null)
            {
                instancesReverse.Add(instance, service);
            }

            if (isResolved)
            {
                TriggerOnRebound(service, instance);
            }

            return instance;
        }

        public bool Release(object mixed)
        {
            if (mixed == null)
            {
                return false;
            }

            string service;
            object instance = null;

            if (!(mixed is string))
            {
                service = GetServiceWithInstanceObject(mixed);
            }
            else
            {
                if (!instances.TryGetValue(service, out instance))
                {
                    // Prevent the use of a string as a service name.
                    service = GetServiceWithInstanceObject(mixed);
                }
            }

            if (instance == null &&
                (string.IsNullOrEmpty(service) || !instances.TryGetValue(service, out instance)))
            {
                return false;
            }

            BindData bindData = GetBindFillable(service);
            TriggerOnRelease(bindData, instance);

            if (instance != null)
            {
                DisposeInstance(instance);
                instancesReverse.Remove(instance);
            }

            instances.Remove(service);

            if (!HasOnReboundCallbacks(service))
            {
                // TODO:
            }

            return true;
        }

        public IContainer OnRelease(Action<IBindData, object> closure)
        {
            AddClosure(closure, release);
            return this;
        }

        public IContainer OnResolving(Action<IBindData, object> closure)
        {
            AddClosure(closure, resolving);
            return this;
        }

        public IContainer OnAfterResolving(Action<IBindData, object> closure)
        {
            AddClosure(closure, afterResloving);
            return this;
        }

        public IContainer OnRebound(string service, Action<object> callback)
        {
            Guard.Requires<ArgumentNullException>(callback != null);
            GuardFlushing();
            if (!IsResolved(service) && !CanMake(service))
            {
                throw new LogicException(
                    $"If you want use Rebound(Watch) , please {nameof(Bind)} or {nameof(Instance)} service first.");
            }

            if (!rebound.TryGetValue(service, out List<Action<object>> list))
            {
                rebound[service] = list = new List<Action<object>>();
            }

            list.Add(callback);
            return this;
        }

        public void Unbind(string service)
        {
            IBindData bind = GetBind(service);
            bind?.Unbind();
        }

        public virtual void Flush()
        {
            try
            {
                flushing = true;
                foreach (var service in instanceTiming.GetIterator(false))
                {
                    Release(service);
                }

                Guard.Requires<AssertException>(instances.Count <= 0);

                instances.Clear();
                bindings.Clear();
                resolving.Clear();
                release.Clear();
                resolved.Clear();
                BuildStack.Clear();
                UserParamsStack.Clear();
                rebound.Clear();
            }
            finally
            {
                flushing = false;
            }
        }

        public string Type2Service(Type type)
        {
            return type.ToString();
        }

        /// <summary>
        /// Trigger all callbacks in specified list.
        /// </summary>
        internal static object Trigger(IBindData bindData, object instance, List<Action<IBindData, object>> list)
        {
            if (list == null)
            {
                return instance;
            }

            foreach (Action<IBindData, object> closure in list)
            {
                closure(bindData, instance);
            }

            return instance;
        }

        internal void Unbind(IBindData bindData)
        {
            GuardFlushing();
            Release(bindData.Service);
            bindings.Remove(bindData.Service);
        }

        protected bool IsBasicType(Type type)
        {
            // IsPrimitive 判断是否为基元类型
            return type == null || type.IsPrimitive || type == typeof(string);
        }

        protected bool IsUnableType(Type type)
        {
            return type == null || type.IsAbstract || type.IsInterface || type.IsArray || type.IsEnum ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        protected virtual string GetParamNeedsService(ParameterInfo baseParam)
        {
            return Type2Service(baseParam.ParameterType);
        }

        protected virtual string GetBuildStackDebugMessage()
        {
            string previous = string.Join(",", BuildStack.ToArray());
            return $"While building stack [{previous}]";
        }

        protected virtual UnresolvableException MakeBuildFailedException(string makeService, Type makeServiceType,
            SException innerException)
        {
            string message = makeServiceType != null
                ? $"Class [{makeServiceType}] build failed. Service is [{makeService}]."
                : $"Service [{makeService}] is not exists.";

            message += GetBuildStackDebugMessage();
            message += GetInnerExceptionMessage(innerException);
            return new UnresolvableException(message, innerException);
        }

        protected virtual string GetInnerExceptionMessage(SException innerException)
        {
            if (innerException == null)
            {
                return string.Empty;
            }

            StringBuilder stack = new StringBuilder();
            do
            {
                if (stack.Length > 0)
                {
                    stack.Append(", ");
                }

                stack.Append(innerException);
            } while ((innerException = innerException.InnerException) != null);

            return $" InnerException mesage stack: [{stack}]";
        }

        protected virtual LogicException MakeCircularDependencyException(string service)
        {
            string message = $"Circular dependency detected while for [{service}].";
            message += GetBuildStackDebugMessage();
            return new LogicException(message);
        }

        protected virtual string FormatService(string service)
        {
            return service.Trim();
        }

        protected virtual void GuardUserParamsCount(int count)
        {
            if (count > 255)
            {
                throw new LogicException(
                    $"Too many parameters, must be less or equal than 255 or override the {nameof(GuardUserParamsCount)} method.");
            }
        }

        /// <summary>
        /// Speculative service type based on specified service name.
        /// </summary>
        protected virtual Type SpeculatedServiceType(string service)
        {
            if (findTypeCache.TryGetValue(service, out Type result))
            {
                return result;
            }

            foreach (Func<string, Type> finder in findType)
            {
                Type type = finder.Invoke(service);
                if (type != null)
                {
                    return findTypeCache[service] = type;
                }
            }

            return findTypeCache[service] = null;
        }

        protected string GetServiceWithInstanceObject(object instance)
        {
            return instancesReverse.TryGetValue(instance, out string origin) ? origin : null;
        }

        protected virtual void GuardServiceName(string service)
        {
            foreach (char c in ServiceBanChars)
            {
                if (service.IndexOf(c) >= 0)
                {
                    throw new LogicException(
                        $"Service name {service} contains disabled characters: {c}. please use Alias replacement"
                    );
                }
            }
        }

        private void GuardFlushing()
        {
            if (flushing)
            {
                throw new LogicException("Container is flushing can not do it.");
            }
        }

        private object TriggerOnResolving(BindData bindData, object instance)
        {
            instance = Trigger(bindData, instance, resolving);
            return TriggerOnAfterResolving(bindData, instance);
        }

        private object TriggerOnAfterResolving(BindData bindData, object instance)
        {
            return Trigger(bindData, instance, afterResloving);
        }

        private void TriggerOnRelease(IBindData bindData, object instance)
        {
            Trigger(bindData, instance, release);
        }

        private void TriggerOnRebound(string service, object instance = null)
        {
            IList<Action<object>> callbacks = GetOnReboundCallbacks(service);
            if (callbacks == null || callbacks.Count <= 0)
            {
                return;
            }

            IBindData bind = GetBind(service);
            instance = instance ?? Make(service);

            for (int index = 0; index < callbacks.Count; index++)
            {
                callbacks[index](instance);

                // If it is a not singleton(static) binding then each callback is given a separate instance.
                if (index + 1 < callbacks.Count && (bind == null || !bind.IsStatic))
                {
                    instance = Make(service);
                }
            }
        }

        private void DisposeInstance(object instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private IList<Action<object>> GetOnReboundCallbacks(string service)
        {
            return rebound.TryGetValue(service, out List<Action<object>> result) ? result : null;
        }

        private bool HasOnReboundCallbacks(string service)
        {
            IList<Action<object>> result = GetOnReboundCallbacks(service);
            return result != null && result.Count > 0;
        }

        private void AddClosure(Action<IBindData, object> closure, List<Action<IBindData, object>> list)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            GuardFlushing();
            list.Add(closure);
        }
    }
}