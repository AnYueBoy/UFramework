namespace UFramework.Core {
    /// <summary>
    /// Contains all events dispatched by an Application.
    /// </summary>
    public static class ApplicationEvents {
        private const string BaseEventArgs = "EventArgs.ApplicationEventArgs";

        public static readonly string OnBeforeBoot = $"{BaseEventArgs}.OnBeforeBoot";

        public static readonly string OnBooting = $"{BaseEventArgs}.BootingEventArgs";

        public static readonly string OnAfterBoot = $"{BaseEventArgs}.AfterBootEventArgs";

        public static readonly string OnRegisterProvider = $"{BaseEventArgs}.RegisterProviderEventArgs";

        public static readonly string OnBeforeInit = $"{BaseEventArgs}.BeforeInitEventArgs";

        public static readonly string OnInitProvider = $"{BaseEventArgs}.InitProviderEventArgs";

        public static readonly string OnAfterInit = $"{BaseEventArgs}.AfterInitEventArgs";

        public static readonly string OnStartCompleted = $"{BaseEventArgs}.StartCompletedEventArgs";

        public static readonly string OnBeforeTerminate = $"{BaseEventArgs}.BeforeTerminateEventArgs";

        public static readonly string OnAfterTerminate = $"{BaseEventArgs}.AfterTerminateEventArgs";

    }
}