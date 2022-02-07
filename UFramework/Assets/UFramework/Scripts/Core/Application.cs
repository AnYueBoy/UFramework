using System;
using System.Collections.Generic;
using System.Threading;
using UFramework.Container;
using UFramework.EventDispatcher;

namespace UFramework.Core {
    public class Application : Container.Container, IApplication {
        private static string version;

        private readonly IList<IServiceProvider> loadedProviders;

        private readonly int mainThreadId;

        private readonly IDictionary<Type, string> dispatchMapping;

        private bool bootstrapped;

        private bool inited;

        private bool registering;

        private long incrementId;

        private DebugLevel debugLevel;

        private IEventDispatcher dispatcher;

        public Application () {
            loadedProviders = new List<IServiceProvider> ();

            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            
        }

        public bool IsMainThread =>
            throw new System.NotImplementedException ();

        public DebugLevel DebugLevel {
            get =>
                throw new System.NotImplementedException ();
            set =>
                throw new System.NotImplementedException ();
        }

        public IEventDispatcher GetDispatcher () {
            throw new System.NotImplementedException ();
        }

        public long GetRuntimeId () {
            throw new System.NotImplementedException ();
        }

        public bool IsRegistered (IServiceProvider provider) {
            throw new System.NotImplementedException ();
        }

        public void Register (IServiceProvider provider) {
            throw new System.NotImplementedException ();
        }

        public void Terminate () {
            throw new System.NotImplementedException ();
        }

      
    }
}