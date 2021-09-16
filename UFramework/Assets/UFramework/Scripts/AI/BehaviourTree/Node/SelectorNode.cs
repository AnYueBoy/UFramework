/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:34:17 
 * @Description: 选择节点（控制流节点）
 */

using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Node;
using UFramework.AI.BlackBoard;

namespace UFramework.AI.BehaviourTree.Node {
    public class SelectorNode : BaseNode {
        private BaseNode m_lastRunningNode;

        protected override RunningStatus onUpdate (IAgent agent, BlackBoardMemory workingMemory) {
            RunningStatus runningStatus = RunningStatus.Finished;
            BaseNode previousNode = m_lastRunningNode;

            // select running node
            m_lastRunningNode = null;
            foreach (BaseNode node in m_Children) {
                runningStatus = node.update (agent, workingMemory);
                if (runningStatus != RunningStatus.Failed) {
                    m_lastRunningNode = node;
                    break;
                }
            }

            // clear last running node
            if (previousNode != m_lastRunningNode && previousNode != null) {
                previousNode.reset (agent, workingMemory);
            }

            return runningStatus;
        }

        protected override void onReset (IAgent agent, BlackBoardMemory workingMemory) {
            if (m_lastRunningNode != null) {
                m_lastRunningNode.reset (agent, workingMemory);
            }

            m_lastRunningNode = null;
        }
    }
}