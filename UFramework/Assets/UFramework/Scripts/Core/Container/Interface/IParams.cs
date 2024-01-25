namespace UFramework
{
    public interface IParams
    {
        bool TryGetValue(string key, out object value);
    }
}