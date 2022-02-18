    using System;
    using SException = System.Exception;
    namespace UFramework.Promise {

        public class RejectHandler {
            public Action<SException> callback;
            public IRejectable rejectable;
        }
    }