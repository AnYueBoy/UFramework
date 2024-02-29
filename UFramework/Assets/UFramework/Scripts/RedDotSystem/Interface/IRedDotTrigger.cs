namespace UFramework
{
    public interface IRedDotTrigger
    {
        string FullPath { get; }
        int TriggerCondition();
    }
}