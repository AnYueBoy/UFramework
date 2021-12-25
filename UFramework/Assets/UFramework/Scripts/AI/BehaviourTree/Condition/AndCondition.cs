/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:43:14 
 * @Description: 与条件(条件节点) 
 */

namespace UFramework.AI.BehaviourTree {

    public class AndCondition : BaseCondition {

        private BaseCondition leftCondistion;
        private BaseCondition rightCondition;

        public AndCondition (BaseCondition leftCondistion, BaseCondition rightCondition) {
            this.leftCondistion = leftCondistion;
            this.rightCondition = rightCondition;
        }

        public override bool isTrue (IAgent agent) {
            return this.leftCondistion.isTrue (agent) && this.rightCondition.isTrue (agent);
        }
    }
}