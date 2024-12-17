using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using SException = System.Exception;

namespace UFramework
{
    public class Container : IContainer
    {
        /// <summary>
        /// 服务名不允许使用的字符
        /// </summary>
        private readonly char[] ServiceBanChars = { '@', ':' };

        /// <summary>
        /// 容器内的所有绑定数据
        /// </summary>
        private readonly Dictionary<string, BindData> bindings;

        /// <summary>
        /// 容器内的所有单例（静态） service-instance
        /// </summary>
        private readonly Dictionary<string, object> instances;

        /// <summary>
        /// 容器内所有单例的反映射 instance-service
        /// </summary>
        private readonly Dictionary<object, string> instancesReverse;

        /// <summary>
        /// 容器内所有注册的tag映射 tag- list
        /// </summary>
        private readonly Dictionary<string, List<string>> tags;

        /// <summary>
        /// 方法的ioc容器
        /// </summary>
        private readonly MethodContainer methodContainer;

        /// <summary>
        /// 表示跳过的对象以跳过某些依赖项注入。
        /// </summary>
        private readonly object skipped;

        /// <summary>
        /// 容器是否正在重置
        /// </summary>
        private bool flushing;

        /// <summary>
        /// 获取栈内当前正在构建的服务
        /// </summary>
        private Stack<string> BuildStack { get; }

        protected Container(int prime = 64)
        {
            prime = Math.Max(8, prime);
            tags = new Dictionary<string, List<string>>((int)(prime * 0.25));
            instances = new Dictionary<string, object>(prime * 4);
            instancesReverse = new Dictionary<object, string>(prime * 4);
            bindings = new Dictionary<string, BindData>(prime * 4);
            BuildStack = new Stack<string>(32);
            skipped = new object();
            methodContainer = new MethodContainer(this);
            flushing = false;
        }

        public object this[string service]
        {
            get => Make(service);
            set
            {
                GetBind(service)?.Unbind();
                Bind(service, (container, args) => value, false);
            }
        }

        private IBindData GetBind(string service)
        {
            if (string.IsNullOrEmpty(service))
            {
                return null;
            }

            if (bindings.TryGetValue(service, out var bindData))
            {
                return bindData;
            }

            return null;
        }

        public IBindData Bind(string service, Func<IContainer, object[], object> concrete, bool isStatic)
        {
            Guard.ParameterNotNull(service, nameof(service));
            Guard.ParameterNotNull(concrete, nameof(concrete));
            GuardServiceName(service);
            GuardFlushing();

            service = service.Trim();
            if (bindings.ContainsKey(service))
            {
                throw new LogicException($"需要添加的绑定的服务{service} 已经存在");
            }

            if (instances.ContainsKey(service))
            {
                throw new LogicException($"单例服务 {service} 已经存在");
            }

            var bindData = new BindData(this, service, concrete, isStatic);
            bindings.Add(service, bindData);
            return bindData;
        }

        public IBindData Bind(string service, Type concrete, bool isStatic)
        {
            Guard.Requires<ArgumentException>(concrete != null, $"参数{concrete} 不能为空");
            if (IsUnableType(concrete))
            {
                throw new LogicException($"类型{concrete} 是不可构建的类型");
            }

            service = service.Trim();
            return Bind(service, WrapperTypeBuilder(service, concrete), isStatic);
        }

        #region 方法绑定

        public IMethodBind BindMethod(string method, object target, MethodInfo called)
        {
            GuardFlushing();
            GuardMethodName(method);
            return methodContainer.Bind(method, target, called);
        }

        public void UnbindMethod(object target)
        {
            methodContainer.Unbind(target);
        }

        public object Invoke(string method, params object[] userParams)
        {
            GuardConstruct(nameof(Invoke));
            return methodContainer.Invoke(method, userParams);
        }

        public object Call(object target, MethodInfo methodInfo, params object[] userParams)
        {
            Guard.Requires<ArgumentNullException>(methodInfo != null);
            if (!methodInfo.IsStatic)
            {
                Guard.Requires<ArgumentNullException>(target != null);
            }

            GuardConstruct(nameof(Call));

            var parameter = methodInfo.GetParameters();
            var bindData = GetBindFillable(target != null ? Type2Service(target.GetType()) : null);
            userParams = GetDependencies(bindData, parameter, userParams) ?? Array.Empty<object>();
            return methodInfo.Invoke(target, userParams);
        }

        #endregion

        public void Tag(string tag, params string[] services)
        {
            Guard.ParameterNotNull(tag, nameof(tag));
            GuardFlushing();

            if (!tags.TryGetValue(tag, out List<string> collection))
            {
                tags[tag] = collection = new List<string>();
            }

            foreach (var service in services ?? Array.Empty<string>())
            {
                if (string.IsNullOrEmpty(service))
                {
                    continue;
                }

                collection.Add(service);
            }
        }

        public object[] Tagged(string tag)
        {
            Guard.ParameterNotNull(tag, nameof(tag));
            if (!tags.TryGetValue(tag, out List<string> services))
            {
                throw new LogicException($"Tag{tag} 不存在");
            }

            return Arr.Map(services, service => Make(service));
        }

        public void Unbind(string service)
        {
            var bind = GetBind(service);
            bind?.Unbind();
        }

        internal void Unbind(IBindable bindable)
        {
            GuardFlushing();
            Release(bindable.Service);
            bindings.Remove(bindable.Service);
        }

        public void Flush()
        {
            try
            {
                flushing = true;
                Guard.Requires<AssertException>(instances.Count <= 0);
                tags.Clear();
                instances.Clear();
                bindings.Clear();
                BuildStack.Clear();
                methodContainer.Flush();
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


        #region 服务实例Make的部分过程

        public object Instance(string service, object instance)
        {
            Guard.ParameterNotNull(service, nameof(service));
            GuardFlushing();
            GuardServiceName(service);

            var bindData = GetBind(service);
            if (bindData != null && !bindData.IsStatic)
            {
                throw new LogicException($"服务{service} 不是静态单例");
            }

            if (instance != null && instancesReverse.TryGetValue(instance, out string realService) &&
                realService != service)
            {
                throw new LogicException($"实例已被注册为单例 服务名：{realService}");
            }

            Release(service);
            instances.Add(service, instance);
            if (instance != null)
            {
                instancesReverse.Add(instance, service);
            }

            return instance;
        }

        protected virtual BindData MakeEmptyBindData(string service)
        {
            return new BindData(this, service, null, false);
        }

        public bool Release(object mixed)
        {
            if (mixed == null)
            {
                return false;
            }

            string service;
            object instance = null;
            if (mixed is string)
            {
                service = mixed.ToString();
                if (!instances.TryGetValue(service, out instance))
                {
                    service = GetServiceWithInstanceObject(mixed);
                }
            }
            else
            {
                service = GetServiceWithInstanceObject(mixed);
            }

            if (instance == null && (string.IsNullOrEmpty(service) || !instances.TryGetValue(service, out instance)))
            {
                return false;
            }

            if (instance != null)
            {
                DisposeInstance(instance);
                instancesReverse.Remove(instance);
            }

            instances.Remove(service);
            return true;
        }

        /// <summary>
        /// 获取服务绑定数据，如果数据为空，则填充数据。
        /// </summary>
        private BindData GetBindFillable(string service)
        {
            return service != null && bindings.TryGetValue(service, out BindData bindData)
                ? bindData
                : MakeEmptyBindData(service);
        }

        /// <summary>
        /// 获取实例对应的服务名称
        /// </summary>
        private string GetServiceWithInstanceObject(object instance)
        {
            return instancesReverse.TryGetValue(instance, out string origin) ? origin : null;
        }

        private void DisposeInstance(object instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public object Make(string service, params object[] userParams)
        {
            GuardConstruct(nameof(Make));
            return Resolve(service, userParams);
        }

        /// <summary>
        /// 确定指定的类型是否是容器的默认基本类型。
        /// </summary>
        protected virtual bool IsBasicType(Type type)
        {
            // IsPrimitive 判断是否为基元类型
            return type == null || type.IsPrimitive || type == typeof(string);
        }

        /// <summary>
        /// 确定指定的类型是否是无法生成的类型。
        /// </summary>
        protected virtual bool IsUnableType(Type type)
        {
            return type == null || type.IsAbstract || type.IsInterface || type.IsArray || type.IsEnum ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// 根据类型创建一个实例生成的闭包
        /// </summary>
        protected virtual Func<IContainer, object[], object> WrapperTypeBuilder(string service, Type concrete)
        {
            return (container, userParams) =>
                ((Container)container).CreateInstance(GetBindFillable(service), concrete, userParams);
        }

        /// <summary>
        /// 解析指定的服务
        /// </summary>
        protected object Resolve(string service, params object[] userParams)
        {
            Guard.ParameterNotNull(service, nameof(service));
            if (instances.TryGetValue(service, out object instance))
            {
                return instance;
            }

            if (BuildStack.Contains(service))
            {
                throw MakeCircularDependencyException(service);
            }

            BuildStack.Push(service);

            try
            {
                var bindData = GetBindFillable(service);
                //我们将开始构建一个服务实例
                //对于已构建的服务，我们将尝试进行依赖项注入
                instance = Build(bindData, userParams);

                if (bindData.IsStatic)
                {
                    instance = Instance(bindData.Service, instance);
                }

                return instance;
            }
            finally
            {
                BuildStack.Pop();
            }
        }

        protected virtual object Build(BindData makeServiceBindData, object[] userParams)
        {
            // 对于构建的服务，如果服务的构建闭包存在则直接使用闭包进行构建实例（性能最高） 否则使用反射构建服务实例
            var instance = makeServiceBindData.Concrete != null
                ? makeServiceBindData.Concrete(this, userParams)
                : CreateInstance(makeServiceBindData, null, userParams);

            // 上述完成构造函数的依赖注入

            // 完成实例的属性注入
            return Inject(makeServiceBindData, instance);
        }

        /// <summary>
        /// 指定实例的依赖项注入。
        /// </summary>
        private object Inject(Bindable bindable, object instance)
        {
            GuardResolveInstance(instance, bindable.Service);

            // 完成属性注入
            AttributeInject(bindable, instance);

            return instance;
        }

        /// <summary>
        /// 属性选择器上(属性)的依赖项注入。
        /// </summary>
        protected virtual void AttributeInject(Bindable makeServiceBindData, object makeServiceInstance)
        {
            if (makeServiceInstance == null)
            {
                return;
            }

            // 获取该实例的所有属性
            var properties = makeServiceInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                // 如果属性不可写入或者属性没有使用注入标签则不进行注入
                if (!property.CanWrite || !property.IsDefined(typeof(InjectAttribute), false))
                {
                    continue;
                }

                var needService = GetPropertyNeedsService(property);

                object instance;
                if (property.PropertyType.IsClass || property.PropertyType.IsInterface)
                {
                    instance = ResolveAttrClass(makeServiceBindData, needService, property);
                }
                else
                {
                    instance = ResolveAttrPrimitive(makeServiceBindData, needService, property);
                }

                // 实例跳过注入
                if (ReferenceEquals(instance, skipped))
                {
                    continue;
                }

                if (!CanInject(property.PropertyType, instance))
                {
                    throw new UnresolvableException(
                        $"{makeServiceBindData.Service}({makeServiceInstance.GetType()}) 的属性注入类型必须是{property.PropertyType}, 但是实例是{instance?.GetType()}, 构建的服务是 {needService}");
                }

                // 属性依赖注入
                property.SetValue(makeServiceInstance, instance, null);
            }
        }

        protected virtual object CreateInstance(Type makeServiceType, object[] userParams)
        {
            // 如果参数不存在，那么在反射时无需写入参数  可获得更好的性能。
            if (userParams == null || userParams.Length <= 0)
            {
                return Activator.CreateInstance(makeServiceType);
            }

            return Activator.CreateInstance(makeServiceType, userParams);
        }


        protected virtual object CreateInstance(Bindable makeServiceBindData, Type makeServiceType, object[] userParams)
        {
            if (IsUnableType(makeServiceType))
            {
                return null;
            }

            // 选择构造函数并根据策略返回该构造函数中参数列表中对应的用户参数数组
            userParams = GetConstructorsInjectParams(makeServiceBindData, makeServiceType, userParams);

            try
            {
                return CreateInstance(makeServiceType, userParams);
            }
            catch (SException e)
            {
                throw MakeBuildFailedException(makeServiceBindData.Service, makeServiceType, e);
            }
        }

        /// <summary>
        /// 选择适当的构造函数并获取相应的参数实例数组。
        /// </summary>
        protected virtual object[] GetConstructorsInjectParams(Bindable makeServiceBindData, Type makeServiceType,
            object[] userParams)
        {
            var constructors = makeServiceType.GetConstructors();
            if (constructors.Length <= 0)
            {
                return Array.Empty<object>();
            }

            ExceptionDispatchInfo exceptionDispatchInfo = null;
            foreach (var constructor in constructors)
            {
                try
                {
                    return GetDependencies(makeServiceBindData, constructor.GetParameters(), userParams);
                }
                catch (SException e)
                {
                    if (exceptionDispatchInfo == null)
                    {
                        exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                    }
                }
            }

            exceptionDispatchInfo?.Throw();
            throw new AssertException("Exception dispatch info is null.");
        }

        /// <summary>
        /// 获取已解析实例的依赖参数数组
        /// </summary>
        protected internal virtual object[] GetDependencies(Bindable makeServiceBindData, ParameterInfo[] baseParams,
            object[] userParams)
        {
            if (baseParams.Length <= 0)
            {
                return Array.Empty<object>();
            }

            var results = new object[baseParams.Length];
            // 获取用于筛选IParams参数的参数匹配器
            var matcher = GetParamsMatcher(ref userParams);

            for (int i = 0; i < baseParams.Length; i++)
            {
                var baseParam = baseParams[i];

                // 参数匹配用于匹配参数。
                // 参数匹配器是第一个执行的，因为它们的匹配精度是最准确的。
                // 从参数匹配器中获取到了匹配参数
                // 优先对 IParams参数进行匹配
                // eg: construct (IParams,object[]) userPrams(object[],IParams)
                var param = matcher?.Invoke(baseParam);

                //当容器发现开发人员使用object或object[]作为
                //依赖参数类型，尝试压缩注入用户参数。
                param ??= GetCompactInjectUserParams(baseParam, ref userParams);

                //从用户参数中选择适当的参数并注入
                //它们按相对顺序排列。
                param ??= GetDependenciesFromUserParams(baseParam, ref userParams);

                string needService = null;

                // eg:construct(IParams,string) userParams(IParams)
                if (param == null)
                {
                    //尝试通过依赖项注入容器生成所需的参数。
                    needService = GetParamNeedsService(baseParam);
                    if (baseParam.ParameterType.IsClass || baseParam.ParameterType.IsInterface)
                    {
                        param = ResolveClass(makeServiceBindData, needService, baseParam);
                    }
                    else
                    {
                        param = ResolvePrimitive(makeServiceBindData, needService, baseParam);
                    }
                }

                if (!CanInject(baseParam.ParameterType, param))
                {
                    var error =
                        $"服务{makeServiceBindData.Service} 参数的注入类型必须是{baseParam.ParameterType}, 但是实例是{param?.GetType()} 类型.";
                    if (needService == null)
                    {
                        error += " 从用户传入参数中注入参数";
                    }
                    else
                    {
                        error += $" 构建的服务是{needService}";
                    }

                    throw new UnresolvableException(error);
                }

                results[i] = param;
            }

            // 根据构造函数的参数与用户参数使用策略匹配后的参数数组
            return results;
        }

        /// <summary>
        /// 获取参数匹配器
        /// </summary>
        protected virtual Func<ParameterInfo, object> GetParamsMatcher(ref object[] userParams)
        {
            if (userParams == null || userParams.Length <= 0)
            {
                return null;
            }

            // 获取用户参数列表中参数类型为IParams的参数数组
            var tables = GetParamsTypeInUserParams(ref userParams);
            return tables.Length <= 0 ? null : MakeParamsMatcher(tables);
        }

        /// <summary>
        /// 生成默认参数IParams匹配器。
        /// </summary>
        private Func<ParameterInfo, object> MakeParamsMatcher(IParams[] tables)
        {
            //默认的匹配器策略将参数名称与参数表的参数名称进行匹配。
            //第一个有效的有效参数值将作为返回值返回 
            return parameterInfo =>
            {
                foreach (var table in tables)
                {
                    if (!table.TryGetValue(parameterInfo.Name, out object result))
                    {
                        continue;
                    }

                    if (ChangeType(ref result, parameterInfo.ParameterType))
                    {
                        return result;
                    }
                }

                return null;
            };
        }

        /// <summary>
        /// 从userParams中获取类型为IParams的变量。
        /// </summary>
        private IParams[] GetParamsTypeInUserParams(ref object[] userParams)
        {
            //此处使用筛选器而不使用Remove，因为IParams也可能是你想要注入的类型之一。
            var elements = Arr.Filter(userParams, value => value is IParams);
            var results = new IParams[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                results[i] = (IParams)elements[i];
            }

            return results;
        }

        /// <summary>
        /// 将实例转换为指定的类型。
        /// </summary>
        protected virtual bool ChangeType(ref object result, Type conversionType)
        {
            try
            {
                // 如果满足下列任一条件，则为 true：
                // 当前 conversionType 位于由 result 表示的对象的继承层次结构中；
                // 当前 conversionType 是 result 实现的接口。
                // 如果不属于其中任一种情况，result 为 null，或者当前 Type 为开放式泛型类型（即 ContainsGenericParameters 返回 true），则为 false。
                if (result == null || conversionType.IsInstanceOfType(result))
                {
                    return true;
                }

                if (IsBasicType(result.GetType()) && conversionType.IsDefined(typeof(VariantAttribute), false))
                {
                    try
                    {
                        result = Make(Type2Service(conversionType), result);
                        return true;
                    }
                    catch (SException e)
                    {
                        // 忽略 当抛出异常时停止注入
                    }
                }

                if (result is IConvertible && typeof(IConvertible).IsAssignableFrom(conversionType))
                {
                    result = Convert.ChangeType(result, conversionType);
                    return true;
                }
            }
            catch (SException e)
            {
                // 忽略 当抛出异常时停止注入
            }

            return false;
        }

        /// <summary>
        /// 获取通过紧缩注入的参数。
        /// </summary>
        protected virtual object GetCompactInjectUserParams(ParameterInfo baseParams, ref object[] userParams)
        {
            if (!CheckCompactInjectUserParams(baseParams, userParams))
            {
                return null;
            }

            try
            {
                if (baseParams.ParameterType == typeof(object) && userParams != null && userParams.Length == 1)
                {
                    return userParams[0];
                }

                return userParams;
            }
            finally
            {
                userParams = null;
            }
        }

        /// <summary>
        /// 检查用户传入的参数是否可以紧缩注入。
        /// </summary>
        protected virtual bool CheckCompactInjectUserParams(ParameterInfo baseParam, object[] userParams)
        {
            if (userParams == null || userParams.Length <= 0)
            {
                return false;
            }

            return baseParam.ParameterType == typeof(object[]) || baseParam.ParameterType == typeof(object);
        }

        /// <summary>
        /// 从用户参数中获取依赖项。
        /// </summary>
        protected virtual object GetDependenciesFromUserParams(ParameterInfo baseParam, ref object[] userParams)
        {
            if (userParams == null)
            {
                return null;
            }

            GuardUserParamsCount(userParams.Length);
            for (int i = 0; i < userParams.Length; i++)
            {
                var userParam = userParams[i];
                if (!ChangeType(ref userParam, baseParam.ParameterType))
                {
                    continue;
                }

                Arr.RemoveAt(ref userParams, i);
                return userParam;
            }

            return null;
        }

        /// <summary>
        /// 将参数类型转为服务名称
        /// </summary>
        protected virtual string GetParamNeedsService(ParameterInfo baseParam)
        {
            return Type2Service(baseParam.ParameterType);
        }

        /// <summary>
        /// 将属性类型转换为服务名称
        /// </summary>
        protected virtual string GetPropertyNeedsService(PropertyInfo propertyInfo)
        {
            return Type2Service(propertyInfo.PropertyType);
        }

        /// <summary>
        /// 解析构造函数的引用类型。
        /// </summary>
        protected virtual object ResolveClass(Bindable makeServiceBindData, string service, ParameterInfo baseParam)
        {
            if (ResolveFromContextual(makeServiceBindData, service, baseParam.ParameterType,
                    out object instance))
            {
                return instance;
            }

            if (baseParam.IsOptional)
            {
                return baseParam.DefaultValue;
            }

            // baseParam.Member可能是空的，这种情况可能是某些底层开发覆盖了ParameterInfo类造成的。
            throw MakeUnresolvableException(baseParam.Name, baseParam.Member?.DeclaringType);
        }

        /// <summary>
        /// 解析构造函数的基元类型。
        /// </summary>
        protected virtual object ResolvePrimitive(Bindable makeServiceBindData, string service, ParameterInfo baseParam)
        {
            if (ResolveFromContextual(makeServiceBindData, service, baseParam.ParameterType,
                    out object instance))
            {
                return instance;
            }

            if (baseParam.IsOptional)
            {
                return baseParam.DefaultValue;
            }

            // 任何可为空的值类型都是泛型 System.Nullable<T> 结构的实例
            if (baseParam.ParameterType.IsGenericType &&
                baseParam.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return null;
            }

            throw MakeUnresolvableException(
                baseParam.Name,
                baseParam.Member?.DeclaringType);
        }

        /// <summary>
        /// 解析属性选择器的引用类型
        /// </summary>
        protected virtual object ResolveAttrClass(Bindable makeServiceBindData, string service, PropertyInfo baseParam)
        {
            if (ResolveFromContextual(makeServiceBindData, service, baseParam.PropertyType,
                    out object instance))
            {
                return instance;
            }

            var inject = (InjectAttribute)baseParam.GetCustomAttribute(typeof(InjectAttribute));
            if (inject != null && !inject.Required)
            {
                return skipped;
            }

            throw MakeUnresolvableException(baseParam.Name, baseParam.DeclaringType);
        }

        /// <summary>
        /// 解析属性选择器的基元类型
        /// </summary>
        protected virtual object ResolveAttrPrimitive(Bindable makeServiceBindData, string service,
            PropertyInfo baseParam)
        {
            if (ResolveFromContextual(makeServiceBindData, service, baseParam.PropertyType,
                    out object instance))
            {
                return instance;
            }

            if (baseParam.PropertyType.IsGenericType &&
                baseParam.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return null;
            }

            var inject = (InjectAttribute)baseParam.GetCustomAttribute(typeof(InjectAttribute));
            if (inject != null && !inject.Required)
            {
                return skipped;
            }

            throw MakeUnresolvableException(baseParam.Name, baseParam.DeclaringType);
        }

        /// <summary>
        /// 根据上下文闭包解析实例
        /// </summary>
        protected virtual bool ResolveFromContextual(Bindable makeServiceBindData, string service, Type paramType,
            out object output)
        {
            if (MakeFromContextualClosure(GetContextualClosure(makeServiceBindData, service), paramType,
                    out output))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 根据闭包中获取实例
        /// </summary>
        protected virtual bool MakeFromContextualClosure(Func<object> closure, Type needType, out object output)
        {
            output = null;
            if (closure == null)
            {
                return false;
            }

            output = closure();
            return ChangeType(ref output, needType);
        }

        /// <summary>
        /// 获取生成委托
        /// </summary>
        protected virtual Func<object> GetContextualClosure(Bindable makeServiceBindData, string service)
        {
            // 获取该服务名称对应的闭包
            return makeServiceBindData.GetContextualClosure(service);
        }

        /// <summary>
        /// 检查指定的实例是否可注入
        /// </summary>
        protected virtual bool CanInject(Type type, object instance)
        {
            return instance == null || type.IsInstanceOfType(instance);
        }

        #endregion

        #region Guard

        private void GuardFlushing()
        {
            if (flushing)
            {
                throw new LogicException("容器正在重置");
            }
        }

        protected virtual void GuardServiceName(string service)
        {
            foreach (var c in ServiceBanChars)
            {
                if (service.IndexOf(c) > 0)
                {
                    throw new LogicException($"服务名称{service} 包含禁用字符 {c},请用别名替代");
                }
            }
        }

        /// <summary>
        /// 验证构造是否可用
        /// </summary>
        protected virtual void GuardConstruct(string method)
        {
        }

        protected virtual void GuardUserParamsCount(int count)
        {
            if (count > 255)
            {
                throw new LogicException($"用户参数的数量必须小于等于255");
            }
        }

        /// <summary>
        /// 确保指定的实例有效。
        /// </summary>
        protected virtual void GuardResolveInstance(object instance, string makeService)
        {
            if (instance == null)
            {
                throw MakeBuildFailedException(makeService, null, null);
            }
        }

        protected virtual void GuardMethodName(string method)
        {
        }

        #endregion

        #region Exception

        /// <summary>
        /// 构建循环依赖报错信息
        /// </summary>
        protected virtual LogicException MakeCircularDependencyException(string service)
        {
            var message = $"检测构建服务时的循环依赖：{service}";
            message += GetBuildStackDebugMessage();
            return new LogicException(message);
        }

        protected virtual string GetBuildStackDebugMessage()
        {
            var previous = string.Join(", ", BuildStack.ToArray());
            return $"生成堆栈前缀信息：{previous}";
        }

        /// <summary>
        /// 创建一个解析失败的异常
        /// </summary>
        protected virtual UnresolvableException MakeUnresolvableException(string name, Type declaringClass)
        {
            return new UnresolvableException(
                $"在解析类{declaringClass?.ToString() ?? "Unknown"} 中的{name ?? "Unknown"}的依赖项时失败");
        }

        protected virtual UnresolvableException MakeBuildFailedException(string makeService, Type makeServiceType,
            SException innerException)
        {
            var message = makeServiceType != null
                ? $"引用类型{makeServiceType}构建失败.服务名称为{makeService}"
                : $"服务{makeService}不存在";

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

            var stack = new StringBuilder();
            do
            {
                if (stack.Length > 0)
                {
                    stack.Append(", ");
                }

                stack.Append(innerException);
            } while ((innerException = innerException.InnerException) != null);

            return $"内部异常栈信息: {stack}";
        }

        #endregion
    }
}