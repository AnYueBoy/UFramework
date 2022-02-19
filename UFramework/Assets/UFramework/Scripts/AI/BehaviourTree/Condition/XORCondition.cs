/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:54:13 
 * @Description: 异或条件
 */

namespace UFramework.AI.BehaviourTree {
    public class XORCondition : BaseCondition {

        private BaseCondition leftCondition;
        private BaseCondition rightCondition;

        public XORCondition (BaseCondition leftCondition, BaseCondition rightCondition) {
            this.leftCondition = leftCondition;
            this.rightCondition = rightCondition;
        }

        public override bool IsTrue (IAgent agent) {
            return this.leftCondition.IsTrue (agent) ^ this.rightCondition.IsTrue (agent);
        }
    }
}