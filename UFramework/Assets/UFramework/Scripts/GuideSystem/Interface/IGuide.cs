namespace UFramework
{
    public interface IGuide
    {
        void InitDataForGuide();

        bool CheckTriggerCondition(float dt);

        void StartGuide();

        void LocalUpdate(float dt);

        void FinishGuide();
    }
}