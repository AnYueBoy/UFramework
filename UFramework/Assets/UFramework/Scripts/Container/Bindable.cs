using System;
using System.Collections.Generic;
using UFramework.Exception;
using UFramework.Util;

namespace UFramework.Container {

    /// <summary>
    /// The bindable data indicates relational data related to the specified service.
    /// </summary>
    public abstract class Bindable : IBindable {

        private readonly Container container;
        private Dictionary<string, string> contextual;
        private Dictionary<string, Func<object>> contextualClosure;
        private bool isDestroy;

        protected Bindable (Container container, string service) {
            this.container = container;
            Service = service;
            isDestroy = false;
        }

        public string Service { get; }

        public IContainer Container => container;

        public void Unbind () {
            isDestroy = true;
            ReleaseBind ();
        }

        /// <summary>
        /// Add the context with service.
        /// </summary>
        internal void AddContextual (string needs, string given) {
            AssertDestroyed ();

            if (contextual == null) {
                contextual = new Dictionary<string, string> ();
            }

            if (contextual.ContainsKey (needs) ||
                (contextualClosure != null && contextualClosure.ContainsKey (needs))) {
                throw new LogicException ($"Needs [{needs}] is already exist.");
            }

            contextual.Add (needs, given);
        }
        internal void AddContextual (string needs, Func<object> given) {
            AssertDestroyed ();

            if (contextualClosure == null) {
                contextualClosure = new Dictionary<string, Func<object>> ();
            }

            if (contextualClosure.ContainsKey (needs) ||
                (contextual != null && contextual.ContainsKey (needs))) {
                throw new LogicException ($"Needs [{needs}] is already exist.");
            }

            contextualClosure.Add (needs, given);
        }

        /// <summary>
        /// Get the demand context of the service.
        /// </summary>
        internal string GetContextual (string needs) {
            if (contextual == null) {
                return null;
            }

            return contextual.TryGetValue (needs, out string contextualNeeds) ? contextualNeeds : null;
        }
        internal Func<object> GetContextualClosure (string needs) {
            if (contextualClosure == null) {
                return null;
            }

            return contextualClosure.TryGetValue (needs, out Func<object> closure) ? closure : null;
        }

        protected abstract void ReleaseBind ();

        protected void AssertDestroyed () {
            if (isDestroy) {
                throw new LogicException ("The current instance is destroyed.");
            }
        }
    }

    public abstract class Bindable<TReturn> : Bindable, IBindable<TReturn>
        where TReturn : class, IBindable<TReturn> {

            protected Bindable (Container container, string service) : base (container, service) { }
        }
}