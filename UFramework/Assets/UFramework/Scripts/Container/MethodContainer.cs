using System;
using System.Collections.Generic;
using System.Reflection;
using UFramework.Exception;
using UFramework.Util;

namespace UFramework.Container {
    /// <summary>
    /// The ioc method container implemented.
    /// </summary>
    internal sealed class MethodContainer {
        private readonly Dictionary<object, List<string>> targetToMethodsMapping;
        private readonly Dictionary<string, MethodBind> methodMapping;
        private readonly Container container;

        internal MethodContainer (Container container) {
            this.container = container;
            targetToMethodsMapping = new Dictionary<object, List<string>> ();
            methodMapping = new Dictionary<string, MethodBind> ();
        }

        /// <summary>
        /// Register a method with the container.
        /// </summary>
        public IMethodBind Bind (string method, object target, MethodInfo methodInfo) {
            Guard.ParameterNotNull (method, nameof (method));
            Guard.ParameterNotNull (methodInfo, nameof (methodInfo));

            if (!methodInfo.IsStatic) {
                Guard.Requires<ArgumentNullException> (target != null);
            }

            if (methodMapping.ContainsKey (method)) {
                throw new LogicException ($"Method[{method}] is already {nameof(Bind)}");
            }

            MethodBind methodBind = new MethodBind (this, container, method, target, methodInfo);
            methodMapping[method] = methodBind;

            if (target == null) {
                return methodBind;
            }

            if (!targetToMethodsMapping.TryGetValue (target, out List<string> targetMappings)) {
                targetToMethodsMapping[target] = targetMappings = new List<string> ();
            }

            targetMappings.Add (method);
            return methodBind;
        }

        /// <summary>
        /// Call the method in bonded container and inject its dependencies.
        /// </summary>
        public object Invoke (string method, params object[] userParams) {
            Guard.ParameterNotNull (method, nameof (method));

            if (!methodMapping.TryGetValue (method, out MethodBind methodBind)) {
                throw MakMethodNotFoundException (method);
            }

            object[] injectParams = container.GetDependencies (methodBind, methodBind.ParameterInfos, userParams) ?? Array.Empty<object> ();
            return methodBind.MethodInfo.Invoke (methodBind.Target, injectParams);
        }

        public void Unbind (object target) {
            Guard.Requires<ArgumentException> (target != null);

            if (target is MethodBind methodBind) {
                methodBind.Unbind ();
                return;
            }

            if (target is string) {
                if (!methodMapping.TryGetValue (target.ToString (), out methodBind)) {
                    return;
                }

                methodBind.Unbind ();
                return;
            }

            UnbindWithObject (target);
        }

        internal void Unbind (MethodBind methodBind) {
            methodMapping.Remove (methodBind.Service);

            if (methodBind.Target == null) {
                return;
            }

            if (!targetToMethodsMapping.TryGetValue (methodBind.Target, out List<string> methods)) {
                return;
            }

            methods.Remove (methodBind.Service);

            if (methods.Count <= 0) {
                targetToMethodsMapping.Remove (methodBind.Target);
            }
        }

        /// <summary>
        /// Flush the container of all method bindings.
        /// </summary>
        public void Flush () {
            targetToMethodsMapping.Clear ();
            methodMapping.Clear ();
        }

        /// <summary>
        /// Create a method without not found exception.
        /// </summary>
        private static LogicException MakMethodNotFoundException (string method) {
            return new LogicException ($"Method [{method}] is not found.");
        }

        /// <summary>
        /// Remove all methods bound to the object.
        /// </summary>
        private void UnbindWithObject (object target) {
            if (!targetToMethodsMapping.TryGetValue (target, out List<string> methods)) {
                return;
            }

            foreach (string method in methods) {
                Unbind (method);
            }
        }
    }
}