/*
 * @Author: l hy 
 * @Date: 2021-03-04 21:32:36 
 * @Description: Promise接口
 * @Last Modified by: l hy
 * @Last Modified time: 2021-03-07 20:12:18
 */

using System;
using SException = System.Exception;
namespace UFramework.Promise {

    public interface IPromise<PromisedT> {

        IPromise<PromisedT> catchs (Action<SException> onRejected);

        void done ();
        void done (Action<PromisedT> onResolved);
        void done (Action<PromisedT> onResolved, Action<SException> onRejected);

        IPromise then (Func<PromisedT, IPromise> onResolved, Action<SException> onRejected);
        IPromise then (Func<PromisedT, IPromise> onResolved);
        IPromise<PromisedT> then (Action<PromisedT> onResolved, Action<SException> onRejected);
        IPromise<PromisedT> then (Action<PromisedT> onResolved);
        IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<SException> onRejected);
        IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved);
    }

    public interface IPromise {

        IPromise catchs (Action<SException> onRejected);

        void done ();
        void done (Action onResolved);
        void done (Action onResolved, Action<SException> onRejected);

        IPromise then (Func<IPromise> onResolved);
        IPromise then (Action onResolved);
        IPromise then (Action onResolved, Action<SException> onRejected);
        IPromise then (Func<IPromise> onResolved, Action<SException> onRejected);
        IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved);
        IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved, Action<SException> onRejected);
    }

}