/*
 * @Author: l hy 
 * @Date: 2021-03-04 21:32:36 
 * @Description: Promise接口
 * @Last Modified by: l hy
 * @Last Modified time: 2021-03-07 20:12:18
 */

namespace UFramework.Promise {
    using System;

    public interface IPromise<PromisedT> {

        IPromise<PromisedT> catchs (Action<Exception> onRejected);

        void done ();
        void done (Action<PromisedT> onResolved);
        void done (Action<PromisedT> onResolved, Action<Exception> onRejected);

        IPromise then (Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected);
        IPromise then (Func<PromisedT, IPromise> onResolved);
        IPromise<PromisedT> then (Action<PromisedT> onResolved, Action<Exception> onRejected);
        IPromise<PromisedT> then (Action<PromisedT> onResolved);
        IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<Exception> onRejected);
        IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved);
    }

    public interface IPromise {

        IPromise catchs (Action<Exception> onRejected);

        void done ();
        void done (Action onResolved);
        void done (Action onResolved, Action<Exception> onRejected);

        IPromise then (Func<IPromise> onResolved);
        IPromise then (Action onResolved);
        IPromise then (Action onResolved, Action<Exception> onRejected);
        IPromise then (Func<IPromise> onResolved, Action<Exception> onRejected);
        IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved);
        IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved, Action<Exception> onRejected);
    }

}