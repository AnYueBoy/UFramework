using SException = System.Exception;

namespace UFramework
{
    public interface IRejectable
    {
        void Reject(SException exception);
    }
}