namespace UFramework.Container {
    public interface IParams {
        /// <summary>
        /// Get paramters by name.
        /// </summary>
        bool TryGetValue (string key, out object value);
    }
}