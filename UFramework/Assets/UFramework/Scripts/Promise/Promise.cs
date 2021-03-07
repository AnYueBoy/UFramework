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

        public int id { get; private set; }

        public string name { get; private set; }

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

        private void invokeRejectHandlers (Exception exception) {
            Action<RejectHandler> fn = null;
            if (this.rejectHandlers != null) {
                if (fn == null) {
                    fn = (RejectHandler handler) => {
                        this.invokeRejectHandler (handler.callback, handler.rejectable, exception);
                    };
                    foreach (RejectHandler rejectHandler in this.rejectHandlers) {
                        fn (rejectHandler);
                    }
                }
            }

            this.clearHandlers ();
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

        private void invokeResolveHandlers () {
            Action<ResolveHandler> fn = null;
            if (this.resolveHandlers != null) {
                if (fn == null) {
                    fn = (ResolveHandler handler) => {
                        this.invokeResolveHandler (handler.callback, handler.rejectable);
                    };
                    foreach (ResolveHandler resolveHandler in this.resolveHandlers) {
                        fn (resolveHandler);
                    }
                }
            }

            this.clearHandlers ();
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

        public static IPromise all (params IPromise[] promises) {
            if (promises.Length == 0) {
                return resolved ();
            }

            int remainingCount = promises.Length;
            Promise resultPromise = new Promise ();
            foreach (IPromise promise in promises) {
                promise
                    .catchs ((Exception exception) => {
                        if (resultPromise.curState == PromiseState.Pending) {
                            resultPromise.reject (exception);
                        }
                    })
                    .then (() => {
                        remainingCount--;
                        if (remainingCount <= 0) {
                            resultPromise.resolve ();
                        }
                    })
                    .done ();
            }
            return resultPromise;
        }

        public IPromise catchs (Action<Exception> onRejected) {
            Promise resultPromise = new Promise ();

            Action resolveHandler = () => {
                resultPromise.resolve ();
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                onRejected (exception);
                resultPromise.reject (exception);
            };

            this.actionHandlers (resultPromise, resolveHandler, rejectHandler);
            return resultPromise;

        }

        private void clearHandlers () {
            this.rejectHandlers = null;
            this.resolveHandlers = null;
        }

        internal static void propagateUnhandledException (object sender, Exception exception) {
            if (unHandledException != null) {
                unHandledException (sender, new ExceptionEventArgs (exception));
            }
        }

        public void done () {
            this.catchs (
                (Exception exception) => {
                    propagateUnhandledException (this, exception);
                }
            );
        }

        public void done (Action onResolved) {
            this.then (onResolved)
                .catchs (
                    (Exception exception) => {
                        propagateUnhandledException (this, exception);
                    }
                );
        }

        public void done (Action onResolved, Action<Exception> onRejected) {
            this.then (onResolved, onRejected)
                .catchs (
                    (Exception exception) => {
                        propagateUnhandledException (this, exception);
                    }
                );
        }

        public static IEnumerable<IPromiseInfo> getPendingPromise () {
            return pendingPromises;
        }

        public void reject (Exception exception) {
            if (this.curState != PromiseState.Pending) {
                throw new ApplicationException (
                    string.Concat (
                        new object[] {
                            "Attempt to reject a promise that is already in state: ",
                            this.curState,
                            ", a promise can only be rejected when it is still in state: ",
                            PromiseState.Pending
                        }));
            }

            this.rejectionException = exception;
            this.curState = PromiseState.Rejected;
            if (enablePromiseTracking) {
                pendingPromises.Remove (this);
            }
            this.invokeRejectHandlers (exception);
        }

        public static IPromise rejected (Exception exception) {
            Promise promise = new Promise ();
            promise.reject (exception);
            return promise;
        }

        public void resolve () {
            if (this.curState != PromiseState.Pending) {
                throw new ApplicationException (
                    string.Concat (
                        new object[] {
                            "Attempt to resolve a promise that is already in state: ",
                            this.curState,
                            ", a promise can only be resolved when it is still in state: ",
                            PromiseState.Pending
                        }));
            }

            this.curState = PromiseState.Resolved;
            if (enablePromiseTracking) {
                pendingPromises.Remove (this);
            }
            this.invokeResolveHandlers ();
        }

        public static IPromise resolved () {
            Promise promise = new Promise ();
            promise.resolve ();
            return promise;
        }

        public IPromise then (Action onResolved) {
            return this.then (onResolved, null);
        }

        public IPromise then (Func<IPromise> onResolved) {
            return this.then (onResolved, null);
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved) {
            return this.then<ConvertedT> (onResolved, null);
        }

        public IPromise then (Action onResolved, Action<Exception> onRejected) {
            Promise resultPromise = new Promise ();
            Action resolveHandler = () => {
                if (onResolved != null) {
                    onResolved ();
                }
                resultPromise.resolve ();
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                if (onRejected != null) {
                    onRejected (exception);
                }
                resultPromise.reject (exception);
            };

            this.actionHandlers (resultPromise, resolveHandler, rejectHandler);
            return resultPromise;
        }

        public IPromise then (Func<IPromise> onResolved, Action<Exception> onRejected) {
            Promise resultPromise = new Promise ();
            Action resolverHandler = () => {
                Action action1 = null;
                Action<Exception> action2 = null;
                if (onResolved != null) {
                    if (action1 == null) {
                        action1 = () => {
                            resultPromise.resolve ();
                        };
                    }

                    if (action2 == null) {
                        action2 = (Exception exception) => {
                            resultPromise.reject (exception);
                        };
                    }

                    onResolved ().then (action1, action2);
                } else {
                    resultPromise.resolve ();
                }
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                if (onRejected != null) {
                    onRejected (exception);
                }
                resultPromise.reject (exception);
            };

            this.actionHandlers (resultPromise, resolverHandler, rejectHandler);
            return resultPromise;
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved, Action<Exception> onRejected) {
            Promise<ConvertedT> resultPromise = new Promise<ConvertedT> ();
            Action resolveHandler = () => {
                onResolved ()
                    .then ((ConvertedT chainedValue) => {
                        resultPromise.resolve (chainedValue);
                    }, (Exception exception) => {
                        resultPromise.reject (exception);
                    });
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                if (onRejected != null) {
                    onRejected (exception);
                }
                resultPromise.reject (exception);
            };

            this.actionHandlers (resultPromise, resolveHandler, rejectHandler);
            return resultPromise;
        }

        public static IPromise race (params IPromise[] promises) {
            if (promises.Length == 0) {
                throw new ApplicationException ("at least 1 input promise must be provided for race");
            }

            Promise resultPromise = new Promise ();
            foreach (IPromise promise in promises) {
                promise
                    .catchs ((Exception exception) => {
                        if (resultPromise.curState == PromiseState.Pending) {
                            resultPromise.reject (exception);
                        }
                    })
                    .then (() => {
                        if (resultPromise.curState == PromiseState.Pending) {
                            resultPromise.resolve ();
                        }
                    })
                    .done ();
            }
            return resultPromise;
        }
    }

    public class Promise<PromisedT> : IPromise<PromisedT>, IPendingPromise<PromisedT>, IRejectable, IPromiseInfo {
        public int id =>
            throw new NotImplementedException ();

        public string name =>
            throw new NotImplementedException ();

        public IPromise<PromisedT> catchs (Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }

        public void done () {
            throw new NotImplementedException ();
        }

        public void done (Action<PromisedT> onResolved) {
            throw new NotImplementedException ();
        }

        public void done (Action<PromisedT> onResolved, Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }

        public void reject (Exception exception) {
            throw new NotImplementedException ();
        }

        public void resolve (PromisedT value) {
            throw new NotImplementedException ();
        }

        public IPromise<PromisedT> then (Action<PromisedT> onResolved) {
            throw new NotImplementedException ();
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved) {
            throw new NotImplementedException ();
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, ConvertedT> transform) {
            throw new NotImplementedException ();
        }

        public IPromise then (Func<PromisedT, IPromise> onResolved) {
            throw new NotImplementedException ();
        }

        public IPromise<PromisedT> then (Action<PromisedT> onResolved, Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }

        public IPromise then (Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<Exception> onRejected) {
            throw new NotImplementedException ();
        }
    }
}