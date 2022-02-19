/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:20:50 
 * @Description: 装饰节点(控制流节点)
 */

namespace UFramework.AI.BehaviourTree {
    public class DecoratorNode : BaseNode {

        public BaseNode child {
            get {
                return m_Children[0];
            }
        }

        public DecoratorNode (BaseNode child) {
            AddChild (child);
        }
    }
}