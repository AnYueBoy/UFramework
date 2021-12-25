/*
 * @Author: l hy 
 * @Date: 2021-01-16 14:44:00 
 * @Description: 动作节点（执行节点）
 */

namespace UFramework.AI.BehaviourTree {
    public class ActionNode : BaseNode {

        private ActionStatus m_actionStauts = ActionStatus.ACTION_READY;

        protected override RunningStatus onUpdate () {
            if (!this.onEvaluate ()) {
                return RunningStatus.Failed;
            }

            RunningStatus runningStatus = RunningStatus.Finished;
            if (m_actionStauts == ActionStatus.ACTION_READY) {
                this.onEnter ();
                m_actionStauts = ActionStatus.ACTION_RUNNING;
            }

            if (m_actionStauts == ActionStatus.ACTION_RUNNING) {
                runningStatus = onExecute ();
            }

            return runningStatus;
        }

        protected override void onReset () {
            if (m_actionStauts == ActionStatus.ACTION_RUNNING) {
                this.onExit ();
            }

            m_actionStauts = ActionStatus.ACTION_READY;
        }

        //implemented by inherited class
        protected virtual bool onEvaluate () {
            return true;
        }

        protected virtual void onEnter () {

        }

        protected virtual RunningStatus onExecute () {
            return RunningStatus.Finished;
        }

        protected virtual void onExit () {

        }
    }
}