/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:34:17 
 * @Description: 选择节点（控制流节点）
 */

namespace UFramework.AI.BehaviourTree {
    public class SelectorNode : BaseNode {
        private BaseNode m_lastRunningNode;

        protected override RunningStatus OnUpdate () {
            RunningStatus runningStatus = RunningStatus.Finished;
            BaseNode previousNode = m_lastRunningNode;

            // select running node
            m_lastRunningNode = null;
            foreach (BaseNode node in m_Children) {
                runningStatus = node.Update (this.agent, this.blackBoardMemory);
                if (runningStatus != RunningStatus.Failed) {
                    m_lastRunningNode = node;
                    break;
                }
            }

            // clear last running node
            if (previousNode != m_lastRunningNode && previousNode != null) {
                previousNode.Reset ();
            }

            return runningStatus;
        }

        protected override void OnReset () {
            foreach (BaseNode node in m_Children) {
                node.Reset ();
            }

            m_lastRunningNode = null;
        }
    }
}