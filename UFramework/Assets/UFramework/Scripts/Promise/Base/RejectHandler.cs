namespace UFramework.Promise {
    using System;

    public class RejectHandler {
        public Action<Exception> callback;
        public IRejectable rejectable;
    }
}