    using SException = System.Exception;
    namespace UFramework.Promise {

        public interface IRejectable {
            void Reject (SException exception);
        }
    }