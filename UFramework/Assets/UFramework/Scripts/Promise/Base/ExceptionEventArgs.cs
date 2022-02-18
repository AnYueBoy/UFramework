    using System;
    using SException = System.Exception;
    namespace UFramework.Promise {
        public class ExceptionEventArgs {

            public SException exception {
                get;
                private set;
            }
            internal ExceptionEventArgs (SException exception) {
                this.exception = exception;
            }
        }
    }