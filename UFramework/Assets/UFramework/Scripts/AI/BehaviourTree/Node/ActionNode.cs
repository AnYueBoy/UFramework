/*
 * @Author: l hy 
 * @Date: 2021-01-16 14:44:00 
 * @Description: 动作节点（执行节点）
 */

namespace UFramework.AI.BehaviourTree {
    public class ActionNode : BaseNode {

        private ActionStatus m_actionStauts = ActionStatus.ACTION_READY;

        protected override RunningStatus OnUpdate () {
            if (!this.OnEvaluate ()) {
                return nodeRunningState = RunningStatus.Failed;
            }

            RunningStatus runningStatus = RunningStatus.Success;
            if (m_actionStauts == ActionStatus.ACTION_READY) {
                this.OnEnter ();
                m_actionStauts = ActionStatus.ACTION_RUNNING;
            }

            if (m_actionStauts == ActionStatus.ACTION_RUNNING) {
                runningStatus = OnExecute ();
            }

            return nodeRunningState = runningStatus;
        }

        protected override void OnReset () {
            if (m_actionStauts == ActionStatus.ACTION_RUNNING) {
                this.OnExit ();
            }

            m_actionStauts = ActionStatus.ACTION_READY;
        }

        //implemented by inherited class
        protected virtual bool OnEvaluate () {
            return true;
        }

        protected virtual void OnEnter () {

        }

        protected virtual RunningStatus OnExecute () {
            return RunningStatus.Success;
        }

        protected virtual void OnExit () {

        }
    }
}