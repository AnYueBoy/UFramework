namespace UFramework.Promise {
    using System.Collections.Generic;
    using System;

    public class Promise : IPromise, IPendingPromise, IRejectable, IPromiseInfo {
        public static bool enablePromiseTracking = false;
        internal static int nextPromiseId = 0;
        internal static HashSet<IPromiseInfo> pendingPromises = new HashSet<IPromiseInfo> ();
        private List<RejectHandler> rejectHandlers;
        private Exception rejectionException;
        private List<ResolveHandler> resolveHandlers = new List<ResolveHandler> ();

        public static event EventHandler<ExceptionEventArgs> unHandledException;

        public PromiseState curState { get; private set; }
        public Promise () {
            this.curState = PromiseState.Pending;
            if (enablePromiseTracking) {
                pendingPromises.Add (this);
            }
        }

        public Promise (Action<Action, Action<Exception>> resolver) {
            Action resolveHandler = null;
            Action<Exception> rejectHandler = null;
            this.curState = PromiseState.Pending;
            if (enablePromiseTracking) {
                pendingPromises.Add (this);
            }

            try {
                if (resolveHandler == null) {
                    resolveHandler = () => this.resolve ();
                }
                if (rejectHandler == null) {
                    rejectHandler = ex => this.reject (ex);
                }

                resolver (resolveHandler, rejectHandler);
            } catch (Exception exception) {
                this.reject (exception);
            }
        }

        private void actionHandlers (IRejectable resultPromise, Action resolveHandler, Action<Exception> rejectHandler) {
            if (this.curState == PromiseState.Resolved) {
                this.invokeResolveHandler (resolveHandler, resultPromise);
            } else if (this.curState == PromiseState.Rejected) {
                this.invokeRejectHandler (rejectHandler, resultPromise, this.rejectionException);
            } else {
                this.addResolveHandler (resolveHandler, resultPromise);
                this.addRejectHandler (rejectHandler, resultPromise);
            }
        }

        private void invokeResolveHandler (Action callback, IRejectable rejectable) {
            try {
                callback ();
            } catch (Exception exception) {

                rejectable.reject (exception);
            }
        }

        private void invokeRejectHandler (Action<Exception> callback, IRejectable rejectable, Exception value) {
            try {
                callback (value);
            } catch (Exception exception) {
                rejectable.reject (exception);
            }
        }

        private void addResolveHandler (Action onResolved, IRejectable rejectable) {
            if (this.resolveHandlers == null) {
                this.resolveHandlers = new List<ResolveHandler> ();
            }

            ResolveHandler item = new ResolveHandler {
                callback = onResolved,
                rejectable = rejectable
            };

            this.resolveHandlers.Add (item);
        }

        private void addRejectHandler (Action<Exception> onRejected, IRejectable rejectable) {
            if (this.rejectHandlers == null) {
                this.rejectHandlers = new List<RejectHandler> ();
            }

            RejectHandler item = new RejectHandler {
                callback = onRejected,
                rejectable = rejectable
            };
            this.rejectHandlers.Add (item);
        }

        public static IPromise all(params IPromise[] promises){
            return all((IEnumerable<IPromise>)promises);
        }

        public static IPromise all (IEnumerable<IPromise> promises) {
            // IPromise[] source = promises;
            return null;
        }

        public int id =>
            throw new NotImplementedException ();

        public string name =>
            throw new NotImplementedException ();

        public IPromise catchs (Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }

        public void done () {
            throw new NotImplementedException ();
        }

        public void done (Action onResolved) {
            throw new NotImplementedException ();
        }

        public void done (Action onResolved, Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }

        public void reject (Exception exception) {
            throw new NotImplementedException ();
        }

        public void resolve () {
            throw new NotImplementedException ();
        }

        public IPromise then (Action onResolved) {
            throw new NotImplementedException ();
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved) {
            throw new NotImplementedException ();
        }

        public IPromise then (Func<IPromise> onResolved) {
            throw new NotImplementedException ();
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved, Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }

        public IPromise then (Action onResolved, Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }

        public IPromise<IEnumerable<ConvertedT>> thenAll<ConvertedT> (Func<IEnumerable<IPromise<ConvertedT>>> chain) {
            throw new NotImplementedException ();
        }

        public IPromise thenAll (Func<IEnumerable<IPromise>> chain) {
            throw new NotImplementedException ();
        }

        public IPromise<ConvertedT> thenRace<ConvertedT> (Func<IEnumerable<IPromise<ConvertedT>>> chain) {
            throw new NotImplementedException ();
        }

        public IPromise thenRace (Func<IEnumerable<IPromise>> chain) {
            throw new NotImplementedException ();
        }
    }
}