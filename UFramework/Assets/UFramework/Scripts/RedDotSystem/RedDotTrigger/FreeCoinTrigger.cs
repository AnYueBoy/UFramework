using GameFramework;

namespace UFramework
{
    public class FreeCoinTrigger : IRedDotTrigger
    {
        public string FullPath => RedDotPath.FreeCoin;

        public int TriggerCondition()
        {
            // TODO： 刷新条件
            return 1;
        }
    }
}