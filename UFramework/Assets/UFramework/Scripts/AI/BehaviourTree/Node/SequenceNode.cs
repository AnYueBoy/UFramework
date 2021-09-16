/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:40:08 
 * @Description: 队列节点（控制流节点）
 */

using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Node;
using UFramework.AI.BlackBoard;

namespace UFramework.AI.BehaviourTree.Node {
    public class SequenceNode : BaseNode {

        private int m_currentNodeIndex = -1;

        protected override RunningStatus onUpdate (IAgent agent, BlackBoardMemory workingMemory) {
            if (m_Children.Count == 0) {
                return RunningStatus.Finished;
            }

            if (m_currentNodeIndex < 0) {
                m_currentNodeIndex = 0;
            }

            for (int i = m_currentNodeIndex; i < m_Children.Count; ++i) {
                RunningStatus status = m_Children[i].update (agent, workingMemory);
                if (status != RunningStatus.Finished) {
                    return status;
                }

                m_currentNodeIndex++;
            }

            return RunningStatus.Finished;
        }

        protected override void onReset (IAgent agent, BlackBoardMemory workingMemory) {
            if (m_currentNodeIndex >= 0 && m_currentNodeIndex < m_Children.Count) {
                m_Children[m_currentNodeIndex].reset (agent, workingMemory);
            }

            m_currentNodeIndex = -1;
        }
    }
}