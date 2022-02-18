namespace UFramework.Core {
    /// <summary>
    /// The framework start process type.
    /// </summary>
    public enum StartProcess {
        /// <summary>
        /// When you create a new Application,
        /// you are in the Construct phase.
        /// </summary>
        Construct = 0,

        /// <summary>
        /// Before the Application.Bootstrap call.
        /// </summary>
        Bootstrap = 1,

        /// <summary>
        /// When during Application.Bootstrap execution,
        /// you are in the Bootstrapping phase.
        /// </summary>
        Bootstrapping = 2,

        /// <summary>
        /// After the Application.Bootstrap called.
        /// </summary>
        Bootstraped = 3,

        /// <summary>
        /// Before the Application.Init call.
        /// </summary>
        Init = 4,

        /// <summary>
        /// When during Application.Init execution,
        /// you are in the Initing phase.
        /// </summary>
        Initing = 5,

        /// <summary>
        /// After the Application.Init called.
        /// </summary>
        Inited = 6,

        /// <summary>
        /// When the framework running.
        /// </summary>
        Running = 7,

        /// <summary>
        /// Before the Application.Terminate call.
        /// </summary>
        Terminate = 8,

        /// <summary>
        /// When during Application.Terminate execution,
        /// you are in the Terminating phase.
        /// </summary>
        Terminating = 9,

        /// <summary>
        /// After the Application.Terminate called.
        /// All resources are destroyed.
        /// </summary>
        Terminated = 10,
    }
}