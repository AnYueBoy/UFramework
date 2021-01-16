/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:35:34 
 * @Description: 条件基类
 */
using UFramework.AI.BehaviourTree.Agent;

namespace UFramework.AI.BehaviourTree.Condition {
    public class BaseCondition {

        public virtual bool isTrue (IAgent agent) {
            return false;
        }
    }
}