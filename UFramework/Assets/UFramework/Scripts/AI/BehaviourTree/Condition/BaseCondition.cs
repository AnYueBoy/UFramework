/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:35:34 
 * @Description: 条件基类
 */
namespace UFramework.AI.BehaviourTree {
    public class BaseCondition {

        public virtual bool IsTrue (IAgent agent) {
            return false;
        }
    }
}