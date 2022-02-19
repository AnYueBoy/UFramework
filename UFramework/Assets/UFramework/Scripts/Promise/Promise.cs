   using System.Collections.Generic;
   using System.Linq;
   using System;
   using SException = System.Exception;
   namespace UFramework.Promise {

       public class Promise : IPromise, IPendingPromise, IRejectable, IPromiseInfo {
           public static bool enablePromiseTracking = false;
           internal static int nextPromiseId = 0;
           private List<RejectHandler> rejectHandlers = new List<RejectHandler> ();
           private List<ResolveHandler> resolveHandlers = new List<ResolveHandler> ();
           private SException rejectionException;

           /* 未处理的异常列表 */
           internal static HashSet<IPromiseInfo> pendingPromises = new HashSet<IPromiseInfo> ();
           public static event EventHandler<ExceptionEventArgs> unHandledException;

           public PromiseState curState { get; private set; }

           public int Id { get; private set; }

           public string Name { get; private set; }

           public Promise () {
               this.Id = ++Promise.nextPromiseId;
               this.curState = PromiseState.Pending;
               if (enablePromiseTracking) {
                   pendingPromises.Add (this);
               }
           }

           public Promise (Action<Action, Action<SException>> resolver) {
               this.Id = ++Promise.nextPromiseId;
               this.curState = PromiseState.Pending;
               if (enablePromiseTracking) {
                   pendingPromises.Add (this);
               }

               try {
                   Action resolveHandler = () => this.Resolve ();
                   Action<SException> rejectHandler = ex => this.Reject (ex);

                   // 创建后执行resolver
                   resolver (resolveHandler, rejectHandler);
               } catch (SException exception) {
                   this.Reject (exception);
               }
           }

           #region promise handler execute
           private void ActionHandlers (IRejectable resultPromise, Action resolveHandler, Action<SException> rejectHandler) {
               if (this.curState == PromiseState.Resolved) {
                   this.ExecuteResolveHandler (resolveHandler, resultPromise);
               } else if (this.curState == PromiseState.Rejected) {
                   this.ExecuteRejectHandler (rejectHandler, resultPromise, this.rejectionException);
               } else {
                   this.AddResolveHandler (resolveHandler, resultPromise);
                   this.AddRejectHandler (rejectHandler, resultPromise);
               }
           }

           private void ExecuteResolveHandler (Action callback, IRejectable rejectable) {
               try {
                   callback ();
               } catch (SException exception) {
                   rejectable.Reject (exception);
               }
           }

           private void ExecuteRejectHandler (Action<SException> callback, IRejectable rejectable, SException value) {
               try {
                   callback (value);
               } catch (SException exception) {
                   rejectable.Reject (exception);
               }
           }

           private void InvokeRejectHandlers (SException exception) {
               foreach (RejectHandler rejectHandler in this.rejectHandlers) {
                   this.ExecuteRejectHandler (rejectHandler.callback, rejectHandler.rejectable, exception);
               }

               this.ClearHandlers ();
           }

           private void InvokeResolveHandlers () {
               foreach (ResolveHandler resolveHandler in this.resolveHandlers) {
                   this.ExecuteResolveHandler (resolveHandler.callback, resolveHandler.rejectable);
               }

               this.ClearHandlers ();
           }

           private void AddResolveHandler (Action onResolved, IRejectable rejectable) {
               ResolveHandler item = new ResolveHandler {
                   callback = onResolved,
                   rejectable = rejectable
               };

               this.resolveHandlers.Add (item);
           }

           private void AddRejectHandler (Action<SException> onRejected, IRejectable rejectable) {
               RejectHandler item = new RejectHandler {
                   callback = onRejected,
                   rejectable = rejectable
               };
               this.rejectHandlers.Add (item);
           }

           private void ClearHandlers () {
               this.rejectHandlers = null;
               this.resolveHandlers = null;
           }

           #endregion

           /* 仅允许在本程序集内访问 */
           internal static void PropagateUnhandledException (object sender, SException exception) {
               // C# 6.0 null 空值操作符
               unHandledException?.Invoke (sender, new ExceptionEventArgs (exception));
           }

           #region  public methond
           public static Promise All (params IPromise[] promises) {
               if (promises.Length == 0) {
                   return Resolved ();
               }

               int remainingCount = promises.Length;
               Promise resultPromise = new Promise ();
               foreach (IPromise promise in promises) {
                   promise
                       .Catchs ((SException exception) => {
                           if (resultPromise.curState == PromiseState.Pending) {
                               resultPromise.Reject (exception);
                           }
                       })
                       .Then (() => {
                           remainingCount--;
                           if (remainingCount <= 0) {
                               resultPromise.Resolve ();
                           }
                       })
                       .Done ();
               }
               return resultPromise;
           }

           public IPromise Catchs (Action<SException> onRejected) {
               Promise resultPromise = new Promise ();

               Action resolveHandler = () => {
                   resultPromise.Resolve ();
               };

               Action<SException> rejectHandler = (SException exception) => {
                   onRejected (exception);
                   resultPromise.Reject (exception);
               };

               this.ActionHandlers (resultPromise, resolveHandler, rejectHandler);
               return resultPromise;

           }

           public void Done () {
               this.Catchs (
                   (SException exception) => {
                       PropagateUnhandledException (this, exception);
                   }
               );
           }

           public void Done (Action onResolved) {
               this.Then (onResolved)
                   .Catchs (
                       (SException exception) => {
                           PropagateUnhandledException (this, exception);
                       }
                   );
           }

           public void Done (Action onResolved, Action<SException> onRejected) {
               this.Then (onResolved, onRejected)
                   .Catchs (
                       (SException exception) => {
                           PropagateUnhandledException (this, exception);
                       }
                   );
           }

           /* 返回未处理异常的迭代器 */
           public static IEnumerable<IPromiseInfo> GetPendingPromise () {
               return pendingPromises;
           }

           public void Reject (SException exception) {
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
               this.InvokeRejectHandlers (exception);
           }

           public static IPromise Rejected (SException exception) {
               Promise promise = new Promise ();
               promise.Reject (exception);
               return promise;
           }

           public void Resolve () {
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
               this.InvokeResolveHandlers ();
           }

           public static Promise Resolved () {
               Promise promise = new Promise ();
               promise.Resolve ();
               return promise;
           }

           public IPromise Then (Action onResolved) {
               return this.Then (onResolved, null);
           }

           public IPromise Then (Func<IPromise> onResolved) {
               return this.Then (onResolved, null);
           }

           public IPromise<ConvertedT> Then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved) {
               return this.Then<ConvertedT> (onResolved, null);
           }

           public IPromise Then (Action onResolved, Action<SException> onRejected) {
               Promise resultPromise = new Promise ();
               Action resolveHandler = () => {
                   onResolved?.Invoke ();
                   resultPromise.Resolve ();
               };

               Action<SException> rejectHandler = (SException exception) => {
                   onRejected?.Invoke (exception);
                   resultPromise.Reject (exception);
               };

               this.ActionHandlers (resultPromise, resolveHandler, rejectHandler);
               return resultPromise;
           }

           /* 此then方法的onResolved回调会携带返回值，但要到的地方较少 */
           public IPromise Then (Func<IPromise> onResolved, Action<SException> onRejected) {
               Promise resultPromise = new Promise ();
               Action resolverHandler = () => {
                   if (onResolved != null) {
                       Action nextResolveHandler = () => {
                           resultPromise.Resolve ();
                       };

                       Action<SException> nextRejectHandler = (SException exception) => {
                           resultPromise.Reject (exception);
                       };

                       onResolved ().Then (nextResolveHandler, nextRejectHandler);
                   } else {
                       resultPromise.Resolve ();
                   }
               };

               Action<SException> rejectHandler = (SException exception) => {
                   onRejected?.Invoke (exception);
                   resultPromise.Reject (exception);
               };

               this.ActionHandlers (resultPromise, resolverHandler, rejectHandler);
               return resultPromise;
           }

           public IPromise<ConvertedT> Then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved, Action<SException> onRejected) {
               Promise<ConvertedT> resultPromise = new Promise<ConvertedT> ();
               Action resolveHandler = () => {
                   onResolved ()
                       .Then ((ConvertedT chainedValue) => {
                           resultPromise.Resolve (chainedValue);
                       }, (SException exception) => {
                           resultPromise.Reject (exception);
                       });
               };

               Action<SException> rejectHandler = (SException exception) => {
                   onRejected?.Invoke (exception);
                   resultPromise.Reject (exception);
               };

               this.ActionHandlers (resultPromise, resolveHandler, rejectHandler);
               return resultPromise;
           }

           /* 竞态状态 */
           public static IPromise Race (params IPromise[] promises) {
               if (promises.Length == 0) {
                   throw new ApplicationException ("at least 1 input promise must be provided for race");
               }

               Promise resultPromise = new Promise ();
               foreach (IPromise promise in promises) {
                   promise
                       .Catchs ((SException exception) => {
                           if (resultPromise.curState == PromiseState.Pending) {
                               resultPromise.Reject (exception);
                           }
                       })
                       .Then (() => {
                           if (resultPromise.curState == PromiseState.Pending) {
                               resultPromise.Resolve ();
                           }
                       })
                       .Done ();
               }
               return resultPromise;
           }

           #endregion
       }

       public class Promise<PromisedT> : IPromise<PromisedT>, IPendingPromise<PromisedT>, IRejectable, IPromiseInfo {

           public int Id { get; private set; }
           public string Name { get; private set; }
           public PromiseState curState { get; private set; }

           private SException rejectionException;
           private List<RejectHandler> rejectHandlers = new List<RejectHandler> ();
           private List<ResolveHandler<PromisedT>> resolveHandlers = new List<ResolveHandler<PromisedT>> ();
           private PromisedT resolveValue;

           public Promise () {
               this.curState = PromiseState.Pending;
               this.Id = ++Promise.nextPromiseId;
               if (Promise.enablePromiseTracking) {
                   Promise.pendingPromises.Add (this);
               }
           }

           public Promise (Action<Action<PromisedT>, Action<SException>> resolver) {
               this.curState = PromiseState.Pending;
               this.Id = ++Promise.nextPromiseId;
               if (Promise.enablePromiseTracking) {
                   Promise.pendingPromises.Add (this);
               }

               try {
                   Action<PromisedT> resolveHandler = (PromisedT value) => {
                       Resolve (value);
                   };

                   Action<SException> rejectHandler = (SException exception) => {
                       Reject (exception);
                   };

                   resolver (resolveHandler, rejectHandler);
               } catch (SException exception) {
                   this.Reject (exception);
               }
           }

           private void ExecuteHandler<T> (Action<T> callback, IRejectable rejectable, T value) {
               try {
                   callback (value);
               } catch (SException exception) {
                   rejectable.Reject (exception);
               }
           }

           private void InvokeRejectHandlers (SException exception) {
               foreach (RejectHandler handler in this.rejectHandlers) {
                   this.ExecuteHandler<SException> (handler.callback, handler.rejectable, exception);
               }

               this.ClearHandlers ();
           }

           private void InvokeResolveHandlers (PromisedT value) {
               foreach (ResolveHandler<PromisedT> handler in this.resolveHandlers) {
                   this.ExecuteHandler<PromisedT> (handler.callback, handler.rejectable, value);
               }
               this.ClearHandlers ();
           }

           private void AddRejectHandler (Action<SException> onRejected, IRejectable rejectable) {
               if (this.rejectHandlers == null) {
                   this.rejectHandlers = new List<RejectHandler> ();
               }

               RejectHandler item = new RejectHandler {
                   callback = onRejected,
                   rejectable = rejectable
               };
               this.rejectHandlers.Add (item);
           }

           private void AddResolveHandler (Action<PromisedT> onResolved, IRejectable rejectable) {
               ResolveHandler<PromisedT> item = new ResolveHandler<PromisedT> {
                   callback = onResolved,
                   rejectable = rejectable
               };

               this.resolveHandlers.Add (item);
           }

           private void ClearHandlers () {
               this.rejectHandlers = null;
               this.resolveHandlers = null;
           }

           private void ActionHandlers (IRejectable resultPromise, Action<PromisedT> resolveHandler, Action<SException> rejectHandler) {
               if (this.curState == PromiseState.Resolved) {
                   this.ExecuteHandler<PromisedT> (resolveHandler, resultPromise, this.resolveValue);
               } else if (this.curState == PromiseState.Rejected) {
                   this.ExecuteHandler<SException> (rejectHandler, resultPromise, this.rejectionException);
               } else {
                   this.AddResolveHandler (resolveHandler, resultPromise);
                   this.AddRejectHandler (rejectHandler, resultPromise);
               }
           }

           public static Promise<PromisedT> Resolved (PromisedT promisedValue) {
               Promise<PromisedT> promise = new Promise<PromisedT> ();
               promise.Resolve (promisedValue);
               return promise;
           }

           public void Resolve (PromisedT value) {
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
               this.InvokeResolveHandlers (value);
           }

           public static IPromise<PromisedT> Rejected (SException exception) {
               Promise<PromisedT> promise = new Promise<PromisedT> ();
               promise.Reject (exception);
               return promise;
           }

           public void Reject (SException exception) {
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
               this.InvokeRejectHandlers (exception);
           }

           public static Promise<IEnumerable<PromisedT>> All (params IPromise<PromisedT>[] promises) {
               if (promises.Length == 0) {
                   return Promise<IEnumerable<PromisedT>>.Resolved (Enumerable.Empty<PromisedT> ());
               }

               int remainingCount = promises.Length;
               PromisedT[] results = new PromisedT[remainingCount];
               Promise<IEnumerable<PromisedT>> resultPromise = new Promise<IEnumerable<PromisedT>> ();

               for (var index = 0; index < promises.Length; index++) {
                   IPromise<PromisedT> promise = promises[index];
                   promise
                       .Catchs ((SException exception) => {
                           if (resultPromise.curState == PromiseState.Pending) {
                               resultPromise.Reject (exception);
                           }
                       })
                       .Then ((PromisedT result) => {
                           results[index] = result;
                           remainingCount--;
                           if (remainingCount <= 0) {
                               resultPromise.Resolve (results);
                           }
                       })
                       .Done ();
               }
               return resultPromise;
           }

           public IPromise<PromisedT> Catchs (Action<SException> onRejected) {
               Promise<PromisedT> resultPromise = new Promise<PromisedT> ();

               Action<PromisedT> resolveHandler = (PromisedT value) => {
                   resultPromise.Resolve (value);
               };

               Action<SException> rejectHandler = (SException exception) => {
                   onRejected (exception);
                   resultPromise.Reject (exception);
               };

               this.ActionHandlers (resultPromise, resolveHandler, rejectHandler);
               return resultPromise;
           }

           public void Done () {
               this.Catchs (
                   (SException exception) => {
                       Promise.PropagateUnhandledException (this, exception);
                   }
               );
           }

           public void Done (Action<PromisedT> onResolved) {
               this.Then (onResolved)
                   .Catchs (
                       (SException exception) => {
                           Promise.PropagateUnhandledException (this, exception);
                       }
                   );
           }

           public void Done (Action<PromisedT> onResolved, Action<SException> onRejected) {
               this.Then (onResolved, onRejected)
                   .Catchs (
                       (SException exception) => {
                           Promise.PropagateUnhandledException (this, exception);
                       }
                   );
           }

           public static IPromise<PromisedT> Race (params IPromise<PromisedT>[] promises) {
               if (promises.Length == 0) {
                   throw new ApplicationException ("At least 1 input promise must be provided for Race");
               }

               Promise<PromisedT> resultPromise = new Promise<PromisedT> ();
               foreach (IPromise<PromisedT> promise in promises) {
                   promise
                       .Catchs ((SException exception) => {
                           if (resultPromise.curState == PromiseState.Pending) {
                               resultPromise.Reject (exception);
                           }
                       })
                       .Then (
                           (PromisedT result) => {
                               if (resultPromise.curState == PromiseState.Pending) {
                                   resultPromise.Resolve (result);
                               }
                           }
                       )
                       .Done ();
               }

               return resultPromise;
           }

           public IPromise<PromisedT> Then (Action<PromisedT> onResolved) {
               return this.Then (onResolved, null);
           }

           public IPromise<ConvertedT> Then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved) {
               return this.Then<ConvertedT> (onResolved, null);
           }

           public IPromise Then (Func<PromisedT, IPromise> onResolved) {
               return this.Then (onResolved, null);
           }

           public IPromise<PromisedT> Then (Action<PromisedT> onResolved, Action<SException> onRejected) {
               Promise<PromisedT> resultPromise = new Promise<PromisedT> ();

               Action<PromisedT> resolveHandler = (PromisedT value) => {
                   onResolved?.Invoke (value);
                   resultPromise.Resolve (value);
               };

               Action<SException> rejectHandler = (SException exception) => {
                   onRejected?.Invoke (exception);
                   resultPromise.Reject (exception);
               };

               this.ActionHandlers (resultPromise, resolveHandler, rejectHandler);
               return resultPromise;
           }

           public IPromise Then (Func<PromisedT, IPromise> onResolved, Action<SException> onRejected) {
               Promise resultPromise = new Promise ();

               Action<PromisedT> resolveHandler = (PromisedT value) => {
                   if (onResolved != null) {
                       Action nextResolveHandler = () => {
                           resultPromise.Resolve ();
                       };

                       Action<SException> nextRejectHandler = (SException exception) => {
                           resultPromise.Reject (exception);
                       };

                       onResolved (value).Then (nextResolveHandler, nextRejectHandler);
                   } else {
                       resultPromise.Resolve ();
                   }
               };

               Action<SException> rejectHandler = (SException exception) => {
                   onRejected?.Invoke (exception);
                   resultPromise.Reject (exception);
               };

               this.ActionHandlers (resultPromise, resolveHandler, rejectHandler);
               return resultPromise;
           }

           public IPromise<ConvertedT> Then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<SException> onRejected) {
               Promise<ConvertedT> resultPromise = new Promise<ConvertedT> ();

               Action<PromisedT> resolveHandler = (PromisedT value) => {
                   onResolved (value)
                       .Then ((ConvertedT chainedValue) => {
                           resultPromise.Resolve (chainedValue);
                       });
               };

               Action<SException> rejectHandler = (SException exception) => {
                   onRejected?.Invoke (exception);
                   resultPromise.Reject (exception);
               };

               this.ActionHandlers (resultPromise, resolveHandler, rejectHandler);

               return resultPromise;
           }
       }
   }