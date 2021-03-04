/*
 * @Author: l hy 
 * @Date: 2021-03-04 21:32:36 
 * @Description: Promise接口
 * @Last Modified by: l hy
 * @Last Modified time: 2021-03-04 22:28:01
 */

namespace UFramework.Promise {
    using System.Collections.Generic;
    using System;

    public interface IPromise<PromisedT> {

        IPromise<PromisedT> catchs (Action<Exception> onRejected);

        void done ();
        void done (Action<PromisedT> onResolved);
        void done (Action<PromisedT> onResolved, Action<Exception> onRejected);

        IPromise<PromisedT> then (Action<PromisedT> onResolved);
        IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved);
        IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, ConvertedT> transform);
        IPromise then (Func<PromisedT, IPromise> onResolved);
        IPromise<PromisedT> then (Action<PromisedT> onResolved, Action<Exception> onRejected);
        IPromise then (Func<PromisedT, IPromise> onResolved, Action<Exception> onRejected);
        IPromise<ConvertedT> then<ConvertedT> (Func<PromisedT, IPromise<ConvertedT>> onResolved, Action<Exception> onRejected);

        IPromise ThenAll (Func<PromisedT, IEnumerable<IPromise>> chain);
        IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT> (Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain);

        IPromise ThenRace (Func<PromisedT, IEnumerable<IPromise>> chain);
        IPromise<ConvertedT> ThenRace<ConvertedT> (Func<PromisedT, IEnumerable<IPromise<ConvertedT>>> chain);
    }

    public interface IPromise {

        IPromise catchs (Action<Exception> onRejected);

        void done ();
        void done (Action onResolved);
        void done (Action onResolved, Action<Exception> onRejected);

        IPromise then (Action onResolved);
        IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved);
        IPromise then (Func<IPromise> onResolved);
        IPromise<ConvertedT> then<ConvertedT> (Func<IPromise<ConvertedT>> onResolved, Action<Exception> onRejected);
        IPromise then (Action onResolved, Action<Exception> onRejected);

        IPromise<IEnumerable<ConvertedT>> thenAll<ConvertedT> (Func<IEnumerable<IPromise<ConvertedT>>> chain);
        IPromise thenAll (Func<IEnumerable<IPromise>> chain);

        IPromise<ConvertedT> thenRace<ConvertedT> (Func<IEnumerable<IPromise<ConvertedT>>> chain);
        IPromise thenRace (Func<IEnumerable<IPromise>> chain);

    }

}