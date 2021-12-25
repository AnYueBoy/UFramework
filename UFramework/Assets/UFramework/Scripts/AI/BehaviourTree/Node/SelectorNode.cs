/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:34:17 
 * @Description: 选择节点（控制流节点）
 */

namespace UFramework.AI.BehaviourTree {
    public class SelectorNode : BaseNode {
        private BaseNode m_lastRunningNode;

        protected override RunningStatus onUpdate () {
            RunningStatus runningStatus = RunningStatus.Finished;
            BaseNode previousNode = m_lastRunningNode;

            // select running node
            m_lastRunningNode = null;
            foreach (BaseNode node in m_Children) {
                runningStatus = node.update (this.agent, this.blackBoardMemory);
                if (runningStatus != RunningStatus.Failed) {
                    m_lastRunningNode = node;
                    break;
                }
            }

            // clear last running node
            if (previousNode != m_lastRunningNode && previousNode != null) {
                previousNode.reset ();
            }

            return runningStatus;
        }

        protected override void onReset () {
            foreach (BaseNode node in m_Children) {
                node.reset ();
            }

            m_lastRunningNode = null;
        }
    }
}