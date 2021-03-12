namespace UFramework.Promise {
    using System;

    public interface IRejectable {
        void reject (Exception exception);
    }
}