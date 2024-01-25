using System;
using System.Collections.Generic;
using System.Reflection;

namespace UFramework
{
    internal sealed class MethodContainer
    {
        // 实例对应的方法组映射
        private readonly Dictionary<object, List<string>> targetToMethodsMappings;

        // 方法名对应的方法绑定数据的映射
        private readonly Dictionary<string, MethodBind> methodMappings;
        private readonly Container container;

        public MethodContainer(Container container)
        {
            this.container = container;
            targetToMethodsMappings = new Dictionary<object, List<string>>();
            methodMappings = new Dictionary<string, MethodBind>();
        }

        /// <summary>
        /// 向容器内注册方法
        /// </summary>
        public IMethodBind Bind(string method, object target, MethodInfo methodInfo)
        {
            Guard.ParameterNotNull(method, nameof(method));
            Guard.ParameterNotNull(methodInfo, nameof(methodInfo));

            if (!methodInfo.IsStatic)
            {
                Guard.Requires<ArgumentNullException>(target != null);
            }

            if (methodMappings.ContainsKey(method))
            {
                throw new LogicException($"方法 {method} 绑定数据已存在");
            }

            var methodBind = new MethodBind(this, container, method, target, methodInfo);
            methodMappings[method] = methodBind;

            if (target == null)
            {
                return methodBind;
            }

            if (!targetToMethodsMappings.TryGetValue(target, out List<string> targetMappings))
            {
                targetToMethodsMappings[target] = targetMappings = new List<string>();
            }

            targetMappings.Add(method);

            return methodBind;
        }

        /// <summary>
        /// 调用容器中的绑定方法并注入其依赖项。
        /// </summary>
        public object Invoke(string method, params object[] userParams)
        {
            Guard.ParameterNotNull(method, nameof(method));

            if (!methodMappings.TryGetValue(method, out MethodBind methodBind))
            {
                throw MakeMethodNotFoundException(method);
            }

            var injectParams = container.GetDependencies(methodBind, methodBind.ParameterInfos, userParams) ??
                               Array.Empty<object>();
            return methodBind.MethodInfo.Invoke(methodBind.Target, injectParams);
        }

        public void Unbind(object target)
        {
            Guard.Requires<ArgumentNullException>(target != null);
            if (target is MethodBind methodBind)
            {
                methodBind.Unbind();
                return;
            }

            if (target is string)
            {
                if (!methodMappings.TryGetValue(target.ToString(), out methodBind))
                {
                    return;
                }

                methodBind.Unbind();
                return;
            }

            UnbindWithObject(target);
        }

        /// <summary>
        /// 删除绑定到对象的所有方法。
        /// </summary>
        private void UnbindWithObject(object target)
        {
            if (!targetToMethodsMappings.TryGetValue(target, out List<string> methods))
            {
                return;
            }

            foreach (var method in methods.ToArray())
            {
                Unbind(method);
            }
        }

        internal void Unbind(MethodBind methodBind)
        {
            methodMappings.Remove(methodBind.Service);
            if (methodBind.Target == null)
            {
                return;
            }

            if (!targetToMethodsMappings.TryGetValue(methodBind.Target, out List<string> methods))
            {
                return;
            }

            methods.Remove(methodBind.Service);
            if (methods.Count <= 0)
            {
                targetToMethodsMappings.Remove(methodBind.Target);
            }
        }

        public void Flush()
        {
            targetToMethodsMappings.Clear();
            methodMappings.Clear();
        }

        #region Exception

        /// <summary>
        /// 创建方法未找到的异常
        /// </summary>
        private static LogicException MakeMethodNotFoundException(string method)
        {
            return new LogicException($"方法 {method} 没有找到");
        }

        #endregion
    }
}