/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:40:08 
 * @Description: 队列节点（控制流节点）
 */

namespace UFramework.AI.BehaviourTree {
    public class SequenceNode : BaseNode {

        private int m_currentNodeIndex = -1;

        protected override RunningStatus OnUpdate () {
            if (m_Children.Count == 0) {
                return nodeRunningState = RunningStatus.Success;
            }

            if (m_currentNodeIndex < 0) {
                m_currentNodeIndex = 0;
            }

            for (int i = m_currentNodeIndex; i < m_Children.Count; ++i) {
                RunningStatus status = m_Children[i].Update (agent, blackBoardMemory, deltaTime);
                if (status != RunningStatus.Success) {
                    return nodeRunningState = status;
                }

                m_currentNodeIndex++;
            }

            return nodeRunningState = RunningStatus.Success;
        }

        protected override void OnReset () {
            foreach (BaseNode node in m_Children) {
                node.Reset ();
            }

            m_currentNodeIndex = -1;
        }
    }
}