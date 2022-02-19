/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:41:10 
 * @Description: 真条件 
 */

namespace UFramework.AI.BehaviourTree {
    public class TrueCondition : BaseCondition {

        public override bool IsTrue (IAgent agent) {
            return true;
        }

    }
}