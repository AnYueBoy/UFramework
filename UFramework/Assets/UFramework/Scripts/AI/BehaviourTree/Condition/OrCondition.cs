/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:43:42 
 * @Description: 或条件 
 */
namespace UFramework.AI.BehaviourTree {
    public class OrCondition : BaseCondition {

        private BaseCondition leftCondition;
        private BaseCondition rightCondition;

        public OrCondition (BaseCondition leftCondition, BaseCondition rightCondition) {
            this.leftCondition = leftCondition;
            this.rightCondition = rightCondition;
        }

        public override bool isTrue (IAgent agent) {
            return this.leftCondition.isTrue (agent) || this.rightCondition.isTrue (agent);
        }
    }
}