namespace UFramework.Promise {
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class Promise : IPromise, IPendingPromise, IRejectable, IPromiseInfo {
        public static bool enablePromiseTracking = false;
        internal static int nextPromiseId = 0;
        private List<RejectHandler> rejectHandlers = new List<RejectHandler> ();
        private List<ResolveHandler> resolveHandlers = new List<ResolveHandler> ();
        private Exception rejectionException;

        /* 未处理的异常列表 */
        internal static HashSet<IPromiseInfo> pendingPromises = new HashSet<IPromiseInfo> ();
        public static event EventHandler<ExceptionEventArgs> unHandledException;

        public PromiseState curState { get; private set; }

        public int id { get; private set; }

        public string name { get; private set; }

        public Promise () {
            this.id = ++Promise.nextPromiseId;
            this.curState = PromiseState.Pending;
            if (enablePromiseTracking) {
                pendingPromises.Add (this);
            }
        }

        public Promise (Action<Action, Action<Exception>> resolver) {
            this.id = ++Promise.nextPromiseId;
            this.curState = PromiseState.Pending;
            if (enablePromiseTracking) {
                pendingPromises.Add (this);
            }

            try {
                Action resolveHandler = () => this.resolve ();
                Action<Exception> rejectHandler = ex => this.reject (ex);

                // 创建后执行resolver
                resolver (resolveHandler, rejectHandler);
            } catch (Exception exception) {
                this.reject (exception);
            }
        }

        #region promise handler execute
        private void actionHandlers (IRejectable resultPromise, Action resolveHandler, Action<Exception> rejectHandler) {
            if (this.curState == PromiseState.Resolved) {
                this.executeResolveHandler (resolveHandler, resultPromise);
            } else if (this.curState == PromiseState.Rejected) {
                this.executeRejectHandler (rejectHandler, resultPromise, this.rejectionException);
            } else {
                this.addResolveHandler (resolveHandler, resultPromise);
                this.addRejectHandler (rejectHandler, resultPromise);
            }
        }

        private void executeResolveHandler (Action callback, IRejectable rejectable) {
            try {
                callback ();
            } catch (Exception exception) {
                rejectable.reject (exception);
            }
        }

        private void executeRejectHandler (Action<Exception> callback, IRejectable rejectable, Exception value) {
            try {
                callback (value);
            } catch (Exception exception) {
                rejectable.reject (exception);
            }
        }

        private void invokeRejectHandlers (Exception exception) {
            foreach (RejectHandler rejectHandler in this.rejectHandlers) {
                this.executeRejectHandler (rejectHandler.callback, rejectHandler.rejectable, exception);
            }

            this.clearHandlers ();
        }

        private void invokeResolveHandlers () {
            foreach (ResolveHandler resolveHandler in this.resolveHandlers) {
                this.executeResolveHandler (resolveHandler.callback, resolveHandler.rejectable);
            }

            this.clearHandlers ();
        }

        private void addResolveHandler (Action onResolved, IRejectable rejectable) {
            ResolveHandler item = new ResolveHandler {
                callback = onResolved,
                rejectable = rejectable
            };

            this.resolveHandlers.Add (item);
        }

        private void addRejectHandler (Action<Exception> onRejected, IRejectable rejectable) {
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

        /* 仅允许在本程序集内访问 */
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

        /* 返回未处理异常的迭代器 */
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

        /* 此then方法的onResolved回调会携带返回值，但要到的地方较少 */
        public IPromise then (Func<IPromise> onResolved, Action<Exception> onRejected) {
            Promise resultPromise = new Promise ();
            Action resolverHandler = () => {
                if (onResolved != null) {
                    Action nextResolveHandler = () => {
                        resultPromise.resolve ();
                    };

                    Action<Exception> nextRejectHandler = (Exception exception) => {
                        resultPromise.reject (exception);
                    };

                    onResolved ().then (nextResolveHandler, nextRejectHandler);
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

        /* 竞态状态 */
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

        private Exception rejectionException;
        private List<RejectHandler> rejectHandlers = new List<RejectHandler> ();
        private List<ResolveHandler<PromisedT>> resolveHandlers = new List<ResolveHandler<PromisedT>> ();
        private PromisedT resolveValue;

        public Promise () {
            this.curState = PromiseState.Pending;
            this.id = ++Promise.nextPromiseId;
            if (Promise.enablePromiseTracking) {
                Promise.pendingPromises.Add (this);
            }
        }

        public Promise (Action<Action<PromisedT>, Action<Exception>> resolver) {
            this.curState = PromiseState.Pending;
            this.id = ++Promise.nextPromiseId;
            if (Promise.enablePromiseTracking) {
                Promise.pendingPromises.Add (this);
            }

            try {
                Action<PromisedT> resolveHandler = (PromisedT value) => {
                    resolve (value);
                };

                Action<Exception> rejectHandler = (Exception exception) => {
                    reject (exception);
                };

                resolver (resolveHandler, rejectHandler);
            } catch (Exception exception) {
                this.reject (exception);
            }
        }

        private void executeHandler<T> (Action<T> callback, IRejectable rejectable, T value) {
            try {
                callback (value);
            } catch (Exception exception) {
                rejectable.reject (exception);
            }
        }

        private void invokeRejectHandlers (Exception exception) {
            foreach (RejectHandler handler in this.rejectHandlers) {
                this.executeHandler<Exception> (handler.callback, handler.rejectable, exception);
            }

            this.clearHandlers ();
        }

        private void invokeResolveHandlers (PromisedT value) {
            foreach (ResolveHandler<PromisedT> handler in this.resolveHandlers) {
                this.executeHandler<PromisedT> (handler.callback, handler.rejectable, value);
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
            ResolveHandler<PromisedT> item = new ResolveHandler<PromisedT> {
                callback = onResolved,
                rejectable = rejectable
            };

            this.resolveHandlers.Add (item);
        }

        private void clearHandlers () {
            this.rejectHandlers = null;
            this.resolveHandlers = null;
        }

        private void actionHandlers (IRejectable resultPromise, Action<PromisedT> resolveHandler, Action<Exception> rejectHandler) {
            if (this.curState == PromiseState.Resolved) {
                this.executeHandler<PromisedT> (resolveHandler, resultPromise, this.resolveValue);
            } else if (this.curState == PromiseState.Rejected) {
                this.executeHandler<Exception> (rejectHandler, resultPromise, this.rejectionException);
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
                if (onResolved != null) {
                    Action nextResolveHandler = () => {
                        resultPromise.resolve ();
                    };

                    Action<Exception> nextRejectHandler = (Exception exception) => {
                        resultPromise.reject (exception);
                    };

                    onResolved (value).then (nextResolveHandler, nextRejectHandler);
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