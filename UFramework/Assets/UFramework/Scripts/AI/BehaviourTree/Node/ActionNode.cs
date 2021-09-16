/*
 * @Author: l hy 
 * @Date: 2021-01-16 14:44:00 
 * @Description: 动作节点（执行节点）
 */

using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Node;
using UFramework.AI.BlackBoard;

namespace UFramework.AI.BehaviourTree.Node {
    public class ActionNode : BaseNode {

        private ActionStatus m_actionStauts = ActionStatus.ACTION_READY;

        protected override RunningStatus onUpdate (IAgent agent, BlackBoardMemory workingMemory) {
            if (!this.onEvaluate (agent, workingMemory)) {
                return RunningStatus.Failed;
            }

            RunningStatus runningStatus = RunningStatus.Finished;
            if (m_actionStauts == ActionStatus.ACTION_READY) {
                this.onEnter (agent, workingMemory);
                m_actionStauts = ActionStatus.ACTION_RUNNING;
            }

            if (m_actionStauts == ActionStatus.ACTION_RUNNING) {
                runningStatus = onExecute (agent, workingMemory);
            }

            return runningStatus;
        }

        protected override void onReset (IAgent agent, BlackBoardMemory workingMemory) {
            if (m_actionStauts == ActionStatus.ACTION_RUNNING) {
                this.onExit (agent, workingMemory);
            }

            m_actionStauts = ActionStatus.ACTION_READY;
        }

        //implemented by inherited class
        protected virtual bool onEvaluate (IAgent agent, BlackBoardMemory workingMemory) {
            return true;
        }

        protected virtual void onEnter (IAgent agent, BlackBoardMemory workingMemory) {

        }

        protected virtual RunningStatus onExecute (IAgent agent, BlackBoardMemory workingMemory) {
            return RunningStatus.Finished;
        }

        protected virtual void onExit (IAgent agent, BlackBoardMemory workingMemory) {

        }
    }
}

