namespace UFramework.Promise {

    public interface IPendingPromise : IRejectable {
        void resolve ();
    }

    public interface IPendingPromise<PromisedT> : IRejectable {
        void resolve (PromisedT value);
    }
}