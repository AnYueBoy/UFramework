namespace UFramework.Promise {
    using System;
    public class ExceptionEventArgs {

        public Exception exception {
            get;
            private set;
        }
        internal ExceptionEventArgs (Exception exception) {
            this.exception = exception;
        }
    }
}