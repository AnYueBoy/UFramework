namespace UFramework.Core.Container
{
    public interface IParams
    {
        bool TryGetValue(string key, out object value);
    }
}