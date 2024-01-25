/*
 * @Author: l hy
 * @Date: 2021-01-16 13:54:13
 * @Description: 异或条件
 */

namespace UFramework
{
    public class XORCondition : BaseCondition
    {
        private BaseCondition leftCondition;
        private BaseCondition rightCondition;

        public XORCondition(BaseCondition leftCondition, BaseCondition rightCondition)
        {
            this.leftCondition = leftCondition;
            this.rightCondition = rightCondition;
        }

        public override bool IsTrue(IAgent agent)
        {
            return leftCondition.IsTrue(agent) ^ rightCondition.IsTrue(agent);
        }
    }
}