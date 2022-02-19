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

        IPromise<PromisedT> Catchs (Action<SException> onRejected);

        void Done ();
        void Done (Action<PromisedT> onResolved);
        void Done (Action<PromisedT> onResolved, Action<SException> onRejected);

        IPromise Then (Func<PromisedT, IPromise> onResolved, Action<SException> onRejected);
        IPromise Then (Func<PromisedT, IPromise> onResolved);
        IPromise<PromisedT> Then (Action<PromisedT> onResolved, Action<SException> onRejected);
        IPromise<PromisedT> Then (Action<PromisedT> onResolved);
        IPromise<ConvertedT> Then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<SException> onRejected);
        IPromise<ConvertedT> Then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved);
    }

    public interface IPromise {

        IPromise Catchs (Action<SException> onRejected);

        void Done ();
        void Done (Action onResolved);
        void Done (Action onResolved, Action<SException> onRejected);

        IPromise Then (Func<IPromise> onResolved);
        IPromise Then (Action onResolved);
        IPromise Then (Action onResolved, Action<SException> onRejected);
        IPromise Then (Func<IPromise> onResolved, Action<SException> onRejected);
        IPromise<ConvertedT> Then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved);
        IPromise<ConvertedT> Then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved, Action<SException> onRejected);
    }

}