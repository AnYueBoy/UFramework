namespace UFramework.AI.BehaviourTree {
    public class InverterNode : DecoratorNode {
        public InverterNode (BaseNode child) : base (child) { }

        protected override RunningStatus OnUpdate () {
            BaseNode childNode = m_Children[0];
            RunningStatus runningStatus = childNode.Update (agent, blackBoardMemory, deltaTime);
            if (runningStatus == RunningStatus.Failed) {
                return nodeRunningState = RunningStatus.Success;
            }

            if (runningStatus == RunningStatus.Success) {
                return nodeRunningState = RunningStatus.Failed;
            }
            return nodeRunningState = runningStatus;
        }

        protected override void OnReset () {
            m_Children[0].Reset ();
        }
    }
}