using System;
using System.Collections.Generic;
using System.Reflection;
using UFramework.Exception;
using UFramework.Util;

namespace UFramework.Core.Container
{
    public class Container : IContainer
    {
        /// <summary>
        /// 服务名不允许使用的字符
        /// </summary>
        private readonly char[] ServiceBanChars = { '@', ':', '$' };

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
        /// 容器内所有的服务别名映射 alias-service
        /// </summary>
        private readonly Dictionary<string, string> aliases;

        /// <summary>
        /// 容器内所有服务别名的反映射 service-list<aliases>
        /// </summary>
        private readonly Dictionary<string, List<string>> aliasesReverse;

        /// <summary>
        /// 容器内所有注册的tag映射 tag- list<service>
        /// </summary>
        private readonly Dictionary<string, List<string>> tags;

        /// <summary>
        /// 所有的全局Resolving回调
        /// </summary>
        private readonly List<Action<IBindData, object>> resolving;

        /// <summary>
        /// 所有的全局afterResolving回调
        /// </summary>
        private readonly List<Action<IBindData, object>> afterResolving;

        /// <summary>
        /// 所有的全局Release回调
        /// </summary>
        private readonly List<Action<IBindData, object>> release;

        /// <summary>
        /// 服务的扩展闭包映射
        /// </summary>
        private readonly Dictionary<string, List<Func<object, IContainer, object>>> extenders;

        /// <summary>
        /// 类型查找器将字符串转换为服务类型。
        /// </summary>
        private readonly SortSet<Func<string, Type>, int> findType;

        /// <summary>
        /// 已找到类型的缓存。
        /// </summary>
        private readonly Dictionary<string, Type> findTypeCache;

        /// <summary>
        /// 已解析的服务的哈希集。
        /// </summary>
        private readonly HashSet<string> resolved;

        /// <summary>
        /// 单例构建时间集合
        /// </summary>
        private readonly SortSet<string, int> instanceTiming;

        /// <summary>
        /// 所有已注册的重绑定的回调
        /// </summary>
        private readonly Dictionary<string, List<Action<object>>> rebound;

        // TODO: 方法的ioc容器

        /// <summary>
        /// 表示跳过的对象以跳过某些依赖项注入。
        /// </summary>
        private readonly object skipped;

        /// <summary>
        /// 容器是否正在重置
        /// </summary>
        private bool flushing;

        /// <summary>
        /// 用于标记全局生成顺序的唯一id。
        /// </summary>
        private int instanceId;

        /// <summary>
        /// 获取栈内当前正在构建的服务
        /// </summary>
        protected Stack<string> BuildStack { get; }

        /// <summary>
        /// 获取栈内当前正在构建的服务的用户参数
        /// </summary>
        protected Stack<object[]> UserParamsStack { get; }

        public Container(int prime = 64)
        {
            prime = Math.Max(8, prime);
            tags = new Dictionary<string, List<string>>((int)(prime * 0.25));
            aliases = new Dictionary<string, string>(prime * 4);
            aliasesReverse = new Dictionary<string, List<string>>(prime * 4);
            instances = new Dictionary<string, object>(prime * 4);
            instancesReverse = new Dictionary<object, string>(prime * 4);
            bindings = new Dictionary<string, BindData>(prime * 4);
            resolving = new List<Action<IBindData, object>>((int)(prime * 0.25));
            afterResolving = new List<Action<IBindData, object>>((int)(prime * 0.25));
            release = new List<Action<IBindData, object>>((int)(prime * 0.25));
            extenders = new Dictionary<string, List<Func<object, IContainer, object>>>((int)(prime * 0.25));
            resolved = new HashSet<string>();
            findType = new SortSet<Func<string, Type>, int>();
            findTypeCache = new Dictionary<string, Type>(prime * 4);
            rebound = new Dictionary<string, List<Action<object>>>(prime);
            instanceTiming = new SortSet<string, int>();
            BuildStack = new Stack<string>(32);
            UserParamsStack = new Stack<object[]>(32);
            skipped = new object();
            // TODO: 方法的ioc容器
            flushing = false;
            instanceId = 0;
        }

        public object this[string service]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IBindData GetBind(string service)
        {
            if (string.IsNullOrEmpty(service))
            {
                return null;
            }

            service = AliasToService(service);
            return bindings.TryGetValue(service, out BindData bindData) ? bindData : null;
        }

        public bool HasBind(string service)
        {
            return GetBind(service) != null;
        }

        public bool HasInstance(string service)
        {
            Guard.ParameterNotNull(service, nameof(service));
            service = AliasToService(service);
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
            service = AliasToService(service);
            if (HasBind(service) || HasInstance(service))
            {
                return true;
            }

            var type = SpeculatedServiceType(service);
            return !IsBasicType(type) && !IsUnableType(type);
        }

        public bool IsStatic(string service)
        {
            var bind = GetBind(service);
            return bind != null && bind.IsStatic;
        }

        public bool IsAlias(string name)
        {
            name = name.Trim();
            return aliases.ContainsKey(name);
        }

        public IContainer Alias(string alias, string service)
        {
            Guard.ParameterNotNull(alias, nameof(alias));
            Guard.ParameterNotNull(service, nameof(service));

            if (alias == service)
            {
                throw new LogicException($"别名与服务名不能一致，{alias}");
            }

            GuardFlushing();

            alias = alias.Trim();
            service = AliasToService(alias);

            if (aliases.ContainsKey(alias))
            {
                throw new LogicException($"别名{alias} 已存在");
            }

            if (bindings.ContainsKey(alias))
            {
                throw new LogicException($"别名{alias}已被用作服务名");
            }

            if (!bindings.ContainsKey(service) && !instances.ContainsKey(service))
            {
                throw new LogicException($"在你使用服务别名前，必须Bind或者Instance一个服务.(简而言之，对服务使用别名之前，这个服务必须存在)");
            }

            aliases.Add(alias, service);
            if (!aliasesReverse.TryGetValue(service, out List<string> collection))
            {
                aliasesReverse[service] = collection = new List<string>();
            }

            collection.Add(alias);

            return this;
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

            if (aliases.ContainsKey(service))
            {
                throw new LogicException($"绑定的服务名称已被别名使用");
            }

            var bindData = new BindData(this, service, concrete, isStatic);
            bindings.Add(service, bindData);
            if (!IsResolved(service))
            {
                return bindData;
            }

            if (isStatic)
            {
                // 如果是“静态”，则直接构建此服务
                Make(service);
            }
            else
            {
                // TODO:
            }

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


        public bool BindIf(string service, Func<IContainer, object[], object> concrete, bool isStatic,
            out IBindData bindData)
        {
            var bind = GetBind(service);
            if (bind == null && (HasInstance(service) || IsAlias(service)))
            {
                bindData = null;
                return false;
            }

            bindData = bind ?? Bind(service, concrete, isStatic);
            return bind == null;
        }

        public bool BindIf(string service, Type concrete, bool isStatic, out IBindData bindData)
        {
            if (!IsUnableType(concrete))
            {
                service = service.Trim();
                return BindIf(service, WrapperTypeBuilder(service, concrete), isStatic, out bindData);
            }

            bindData = null;
            return false;
        }

        #region Method

        public IMethodBind BindMethod(string method, object target, MemberInfo called)
        {
            throw new NotImplementedException();
        }

        public void UnbindMethod(object target)
        {
            throw new NotImplementedException();
        }

        public object Invoke(string method, params object[] userParams)
        {
            throw new NotImplementedException();
        }

        public object Call(object target, MethodInfo methodInfo, params object[] userParams)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Unbind()
        {
            throw new NotImplementedException();
        }

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

        public object Instance(string service, object instance)
        {
            throw new NotImplementedException();
        }

        public bool Release(object mixed)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public object Make(string service, params object[] userParams)
        {
            throw new NotImplementedException();
        }

        public void Extend(string service, Func<object, IContainer, object> closure)
        {
            throw new NotImplementedException();
        }

        public IContainer OnResolving(Action<IBindData, object> closure)
        {
            throw new NotImplementedException();
        }

        public IContainer OnAfterResolving(Action<IBindData, object> closure)
        {
            throw new NotImplementedException();
        }

        public IContainer OnRelease(Action<IBindData, object> closure)
        {
            throw new NotImplementedException();
        }

        public IContainer OnFindType(Func<string, Type> func, int priority = Int32.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IContainer OnRebound(string service, Action<object> callback)
        {
            throw new NotImplementedException();
        }

        public string Type2Service(Type type)
        {
            return type.ToString();
        }

        /// <summary>
        /// 将别名转为服务名称
        /// </summary>
        private string AliasToService(string name)
        {
            name = name.Trim();
            return aliases.TryGetValue(name, out string service) ? service : name;
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
        /// 根据指定服务名称来推测服务类型。
        /// </summary>
        protected virtual Type SpeculatedServiceType(string service)
        {
            if (findTypeCache.TryGetValue(service, out Type result))
            {
                return result;
            }

            foreach (var finder in findType)
            {
                var type = finder.Invoke(service);
                if (type != null)
                {
                    return findTypeCache[service] = type;
                }
            }

            return findTypeCache[service] = null;
        }

        /// <summary>
        /// 根据类型创建一个实例生成的闭包
        /// </summary>
        /// <param name="service"></param>
        /// <param name="concrete"></param>
        /// <returns></returns>
        protected virtual Func<IContainer, object[], object> WrapperTypeBuilder(string service, Type concrete)
        {
            // TODO:
            return null;
        }

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
    }
}