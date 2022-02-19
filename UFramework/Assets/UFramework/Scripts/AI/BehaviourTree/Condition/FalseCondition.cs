/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:41:49 
 * @Description: false条件
 */

namespace UFramework.AI.BehaviourTree {
    public class FalseCondition : BaseCondition {

        public override bool IsTrue (IAgent agent) {
            return false;
        }
    }
}