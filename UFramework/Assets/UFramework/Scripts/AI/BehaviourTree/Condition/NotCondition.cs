/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:57:05 
 * @Description: 非条件
 */

namespace UFramework.AI.BehaviourTree {

    public class NotCondition : BaseCondition {

        private BaseCondition condistion;

        public NotCondition (BaseCondition condition) {
            this.condistion = condition;
        }

        public override bool IsTrue (IAgent agent) {
            return !this.condistion.IsTrue (agent);
        }
    }
}