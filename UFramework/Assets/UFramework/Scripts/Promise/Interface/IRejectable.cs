    using SException = System.Exception;
    namespace UFramework.Promise {

        public interface IRejectable {
            void reject (SException exception);
        }
    }