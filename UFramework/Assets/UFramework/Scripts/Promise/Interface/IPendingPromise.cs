namespace UFramework
{
    public interface IPendingPromise : IRejectable
    {
        void Resolve();
    }

    public interface IPendingPromise<PromisedT> : IRejectable
    {
        void Resolve(PromisedT value);
    }
}