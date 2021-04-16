namespace UFramework.Promise {
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class Promise : IPromise, IPendingPromise, IRejectable, IPromiseInfo {
        public static bool enablePromiseTracking = false;
        internal static int nextPromiseId = 0;
        internal static HashSet<IPromiseInfo> pendingPromises = new HashSet<IPromiseInfo> ();
        private List<RejectHandler> rejectHandlers;
        private List<ResolveHandler> resolveHandlers = new List<ResolveHandler> ();
        private Exception rejectionException;
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

                // 创建后执行resolver
                resolver (resolveHandler, rejectHandler);
            } catch (Exception exception) {
                this.reject (exception);
            }
        }

        #region promise handler execute
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

        private void clearHandlers () {
            this.rejectHandlers = null;
            this.resolveHandlers = null;
        }

        #endregion

        internal static void propagateUnhandledException (object sender, Exception exception) {
            // C# 6.0 null 空值操作符
            unHandledException?.Invoke (sender, new ExceptionEventArgs (exception));
        }

        #region  public methond
        public static Promise all (params IPromise[] promises) {
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

        public static Promise resolved () {
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
                onResolved?.Invoke ();
                resultPromise.resolve ();
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                onRejected?.Invoke (exception);
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
                onRejected?.Invoke (exception);
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
                onRejected?.Invoke (exception);
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

        #endregion
    }

    public class Promise<PromisedT> : IPromise<PromisedT>, IPendingPromise<PromisedT>, IRejectable, IPromiseInfo {

        public int id { get; private set; }
        public string name { get; private set; }
        public PromiseState curState { get; private set; }

        private List<RejectHandler> rejectHandlers;
        private Exception rejectionException;
        private List<Action<PromisedT>> resolveCallbacks;
        private List<IRejectable> resolveRejectables;
        private PromisedT resolveValue;

        public Promise () {
            this.curState = PromiseState.Pending;
            this.id = ++Promise.nextPromiseId;
            if (Promise.enablePromiseTracking) {
                Promise.pendingPromises.Add (this);
            }
        }

        public Promise (Action<Action<PromisedT>, Action<Exception>> resolver) {
            Action<PromisedT> action = null;
            Action<Exception> action2 = null;
            this.curState = PromiseState.Pending;
            this.id = ++Promise.nextPromiseId;
            if (Promise.enablePromiseTracking) {
                Promise.pendingPromises.Add (this);
            }

            try {
                if (action == null) {
                    action = (PromisedT value) => {
                        resolve (value);
                    };
                }

                if (action2 == null) {
                    action2 = (Exception exception) => {
                        reject (exception);
                    };
                }

                resolver (action, action2);
            } catch (Exception exception) {
                this.reject (exception);
            }
        }

        private void invokeHandler<T> (Action<T> callback, IRejectable rejectable, T value) {
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
                        this.invokeHandler<Exception> (handler.callback, handler.rejectable, exception);
                    };
                }

                foreach (RejectHandler handler in this.rejectHandlers) {
                    fn (handler);
                }
            }

            this.clearHandlers ();
        }

        private void invokeResolveHandlers (PromisedT value) {
            if (this.resolveCallbacks != null) {
                int num = 0;
                int count = this.resolveCallbacks.Count;
                while (num < count) {
                    this.invokeHandler<PromisedT> (this.resolveCallbacks[num], this.resolveRejectables[num], value);
                    num++;
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

        private void addResolveHandler (Action<PromisedT> onResolved, IRejectable rejectable) {
            if (this.resolveCallbacks == null) {
                this.resolveCallbacks = new List<Action<PromisedT>> ();
            }
            if (this.resolveRejectables == null) {
                this.resolveRejectables = new List<IRejectable> ();
            }
            this.resolveCallbacks.Add (onResolved);
            this.resolveRejectables.Add (rejectable);
        }

        private void clearHandlers () {
            this.rejectHandlers = null;
            this.resolveCallbacks = null;
            this.resolveRejectables = null;
        }

        private void actionHandlers (IRejectable resultPromise, Action<PromisedT> resolveHandler, Action<Exception> rejectHandler) {
            if (this.curState == PromiseState.Resolved) {
                this.invokeHandler<PromisedT> (resolveHandler, resultPromise, this.resolveValue);
            } else if (this.curState == PromiseState.Rejected) {
                this.invokeHandler<Exception> (rejectHandler, resultPromise, this.rejectionException);
            } else {
                this.addResolveHandler (resolveHandler, resultPromise);
                this.addRejectHandler (rejectHandler, resultPromise);
            }
        }

        public static Promise<PromisedT> resolved (PromisedT promisedValue) {
            Promise<PromisedT> promise = new Promise<PromisedT> ();
            promise.resolve (promisedValue);
            return promise;
        }

        public void resolve (PromisedT value) {
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

            this.resolveValue = value;
            this.curState = PromiseState.Resolved;
            if (Promise.enablePromiseTracking) {
                Promise.pendingPromises.Remove (this);
            }
            this.invokeResolveHandlers (value);
        }

        public static IPromise<PromisedT> rejected (Exception exception) {
            Promise<PromisedT> promise = new Promise<PromisedT> ();
            promise.reject (exception);
            return promise;
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
            if (Promise.enablePromiseTracking) {
                Promise.pendingPromises.Remove (this);
            }
            this.invokeRejectHandlers (exception);
        }

        public static Promise<IEnumerable<PromisedT>> all (params IPromise<PromisedT>[] promises) {
            if (promises.Length == 0) {
                return Promise<IEnumerable<PromisedT>>.resolved (Enumerable.Empty<PromisedT> ());
            }

            int remainingCount = promises.Length;
            PromisedT[] results = new PromisedT[remainingCount];
            Promise<IEnumerable<PromisedT>> resultPromise = new Promise<IEnumerable<PromisedT>> ();

            for (var index = 0; index < promises.Length; index++) {
                IPromise<PromisedT> promise = promises[index];
                // FIXME: 可能会有问题，原版本使用的是Each循环(C# 新版未找到Each循环)
                promise
                    .catchs ((Exception exception) => {
                        if (resultPromise.curState == PromiseState.Pending) {
                            resultPromise.reject (exception);
                        }
                    })
                    .then ((PromisedT result) => {
                        results[index] = result;
                        remainingCount--;
                        if (remainingCount <= 0) {
                            resultPromise.resolve (results);
                        }
                    })
                    .done ();
            }
            return resultPromise;
        }

        public IPromise<PromisedT> catchs (Action<Exception> onRejected) {
            Promise<PromisedT> resultPromise = new Promise<PromisedT> ();

            Action<PromisedT> resolveHandler = (PromisedT value) => {
                resultPromise.resolve (value);
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                onRejected (exception);
                resultPromise.reject (exception);
            };

            this.actionHandlers (resultPromise, resolveHandler, rejectHandler);
            return resultPromise;
        }

        public void done () {
            this.catchs (
                (Exception exception) => {
                    Promise.propagateUnhandledException (this, exception);
                }
            );
        }

        public void done (Action<PromisedT> onResolved) {
            this.then (onResolved)
                .catchs (
                    (Exception exception) => {
                        Promise.propagateUnhandledException (this, exception);
                    }
                );
        }

        public void done (Action<PromisedT> onResolved, Action<Exception> onRejected) {
            this.then (onResolved, onRejected)
                .catchs (
                    (Exception exception) => {
                        Promise.propagateUnhandledException (this, exception);
                    }
                );
        }

        public static IPromise<PromisedT> race (params IPromise<PromisedT>[] promises) {
            if (promises.Length == 0) {
                throw new ApplicationException ("At least 1 input promise must be provided for Race");
            }

            Promise<PromisedT> resultPromise = new Promise<PromisedT> ();
            foreach (IPromise<PromisedT> promise in promises) {
                promise
                    .catchs ((Exception exception) => {
                        if (resultPromise.curState == PromiseState.Pending) {
                            resultPromise.reject (exception);
                        }
                    })
                    .then (
                        (PromisedT result) => {
                            if (resultPromise.curState == PromiseState.Pending) {
                                resultPromise.resolve (result);
                            }
                        }
                    )
                    .done ();
            }

            return resultPromise;
        }

        public IPromise<PromisedT> then (Action<PromisedT> onResolved) {
            return this.then (onResolved, null);
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved) {
            return this.then<ConvertedT> (onResolved, null);
        }

        public IPromise then (Func<PromisedT, IPromise> onResolved) {
            return this.then (onResolved, null);
        }

        public IPromise<PromisedT> then (Action<PromisedT> onResolved, Action<Exception> onRejected) {
            Promise<PromisedT> resultPromise = new Promise<PromisedT> ();

            Action<PromisedT> resolveHandler = (PromisedT value) => {
                onResolved?.Invoke (value);
                resultPromise.resolve (value);
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                onRejected?.Invoke (exception);
                resultPromise.reject (exception);
            };

            this.actionHandlers (resultPromise, resolveHandler, rejectHandler);
            return resultPromise;
        }

        public IPromise then (Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected) {
            Promise resultPromise = new Promise ();

            Action<PromisedT> resolveHandler = (PromisedT value) => {
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

                    onResolved (value).then (action1, action2);
                } else {
                    resultPromise.resolve ();
                }
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                onRejected?.Invoke (exception);
                resultPromise.reject (exception);
            };

            this.actionHandlers (resultPromise, resolveHandler, rejectHandler);
            return resultPromise;
        }

        public IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<Exception> onRejected) {
            Promise<ConvertedT> resultPromise = new Promise<ConvertedT> ();

            Action<PromisedT> resolveHandler = (PromisedT value) => {
                onResolved (value)
                    .then ((ConvertedT chainedValue) => {
                        resultPromise.resolve (chainedValue);
                    });
            };

            Action<Exception> rejectHandler = (Exception exception) => {
                onRejected?.Invoke (exception);
                resultPromise.reject (exception);
            };

            this.actionHandlers (resultPromise, resolveHandler, rejectHandler);

            return resultPromise;
        }
    }
}