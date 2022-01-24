using System.Reflection;
namespace UFramework.Container {
    internal sealed class MethodBind : Bindable<IMethodBind>, IMethodBind {

        private readonly MethodContainer methodContainer;

        /// <summary>
        /// Gets the instance on which to call the method.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Gets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets an array of the method parameters.
        /// </summary>
        public ParameterInfo[] ParameterInfos { get; }

        public MethodBind (
            MethodContainer methodContainer,
            Container container,
            string service,
            object target,
            MethodInfo call) : base (container, service) {
            this.methodContainer = methodContainer;
            Target = target;
            MethodInfo = call;
            ParameterInfos = call.GetParameters ();
        }

        /// <summary>
        /// Unbinds a method from the container.
        /// </summary>
        protected override void ReleaseBind () {
            methodContainer.Unbind (this);
        }
    }
}