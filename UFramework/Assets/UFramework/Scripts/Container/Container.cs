using System;
using System.Collections.Generic;
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
        private readonly HashSet<string> resolved;

        private readonly Dictionary<object, string> instancesReverse;
        private readonly List<Action<IBindData, object>> resolving;
        private readonly List<Action<IBindData, object>> afterResloving;
        private readonly List<Action<IBindData, object>> release;

        private readonly SortSet<Func<string, Type>, int> findType;
        private readonly Dictionary<string, Type> findTypeCache;

        private bool flushing;
        private Stack<string> BuildStack { get; }
        private Stack<object[]> UserParamsStack { get; }

        protected Container(int prime = 64)
        {
            prime = Math.Max(8, prime);
            instances = new Dictionary<string, object>(prime * 4);
            instancesReverse = new Dictionary<object, string>(prime * 4);
            bindings = new Dictionary<string, BindData>(prime * 4);
            resolved = new HashSet<string>();

            resolving = new List<Action<IBindData, object>>((int) (prime * 0.25));
            afterResloving = new List<Action<IBindData, object>>((int) (prime * 0.25));
            release = new List<Action<IBindData, object>>((int) (prime * 0.25));

            findType = new SortSet<Func<string, Type>, int>();
            findTypeCache = new Dictionary<string, Type>(prime * 4);

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

        private Func<IContainer, object[], object> WrapperTypeBuilder(string service, Type concrete)
        {
            return (container, userParams) =>
                ((Container) container).CreateInstance(GetBindFillable(service), concrete, userParams);
        }

        private BindData GetBindFillable(string service)
        {
            return service != null && bindings.TryGetValue(service, out BindData bindData)
                ? bindData
                : MakeEmptyBindData(service);
        }

        protected BindData MakeEmptyBindData(string service)
        {
            return new BindData(service, null, false);
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
            BindData bindData = new BindData(service, concrete, isStatic);
            bindings.Add(service, bindData);

            if (!IsResolved(service))
            {
                return bindData;
            }

            if (isStatic)
            {
                Make(service);
            }

            return bindData;
        }

        public object Make(string service, params object[] userParams)
        {
            GuardConstruct(nameof(Make));
            return Resolve(service, userParams);
        }

        protected virtual void GuardConstruct(string method)
        {
        }

        private object Resolve(string service, params object[] userParams)
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

        private object Build(BindData makeServiceBindData, object[] userParams)
        {
            object instance = makeServiceBindData.Concrete != null
                ? makeServiceBindData.Concrete(this, userParams)
                : CreateInstance(makeServiceBindData, SpeculatedServiceType(makeServiceBindData.Service), userParams);

            return instance;
        }

        private object CreateInstance(BindData makeServiceBindData, Type makeServiceType, object[] userParams)
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

        private object CreateInstance(Type makeService, object[] userParams)
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

            Release(service);

            instances.Add(service, instance);

            if (instance != null)
            {
                instancesReverse.Add(instance, service);
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
                service = mixed as string;
                instances.TryGetValue(service, out instance);
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

        public void Unbind(string service)
        {
            IBindData bind = GetBind(service);
            Unbind(bind);
        }

        public void Flush()
        {
            try
            {
                flushing = true;
                foreach (var service in instances.Keys)
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

        private static object Trigger(IBindData bindData, object instance, List<Action<IBindData, object>> list)
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

        private void Unbind(IBindData bindData)
        {
            if (bindData == null)
            {
                return;
            }

            GuardFlushing();
            Release(bindData.Service);
            bindings.Remove(bindData.Service);
        }

        private bool IsBasicType(Type type)
        {
            // IsPrimitive 判断是否为基元类型
            return type == null || type.IsPrimitive || type == typeof(string);
        }

        private bool IsUnableType(Type type)
        {
            return type == null || type.IsAbstract || type.IsInterface || type.IsArray || type.IsEnum ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }


        private LogicException MakeCircularDependencyException(string service)
        {
            string message = $"Circular dependency detected while for [{service}].";
            message += GetBuildStackDebugMessage();
            return new LogicException(message);
        }

        private string GetBuildStackDebugMessage()
        {
            string previous = string.Join(",", BuildStack.ToArray());
            return $"While building stack [{previous}]";
        }

        private UnresolvableException MakeBuildFailedException(string makeService, Type makeServiceType,
            SException innerException)
        {
            string message = makeServiceType != null
                ? $"Class [{makeServiceType}] build failed. Service is [{makeService}]."
                : $"Service [{makeService}] is not exists.";

            message += GetBuildStackDebugMessage();
            message += GetInnerExceptionMessage(innerException);
            return new UnresolvableException(message, innerException);
        }

        private string GetInnerExceptionMessage(SException innerException)
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

        protected string FormatService(string service)
        {
            return service.Trim();
        }

        public IContainer OnFindType(Func<string, Type> func, int priority = int.MaxValue)
        {
            Guard.Requires<ArgumentNullException>(func != null);
            GuardFlushing();
            findType.Add(func, priority);
            return this;
        }

        protected Type SpeculatedServiceType(string service)
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

        private string GetServiceWithInstanceObject(object instance)
        {
            return instancesReverse.TryGetValue(instance, out string origin) ? origin : null;
        }

        private void GuardServiceName(string service)
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

        private void DisposeInstance(object instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void AddClosure(Action<IBindData, object> closure, List<Action<IBindData, object>> list)
        {
            Guard.Requires<ArgumentNullException>(closure != null);
            GuardFlushing();
            list.Add(closure);
        }
    }
}