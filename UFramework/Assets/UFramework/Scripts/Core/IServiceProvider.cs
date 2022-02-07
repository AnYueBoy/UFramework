namespace UFramework.Core {
    public interface IServiceProvider {

        /// <summary>
        /// Initialize the application's service providers.
        /// </summary>
        void Init ();

        /// <summary>
        /// Register any application services.
        /// </summary>
        void Register ();
    }
}