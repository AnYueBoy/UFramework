namespace UFramework.AI.BehaviourTree {
    public class SuccessNode : DecoratorNode {
        public SuccessNode (BaseNode child) : base (child) { }

        protected override RunningStatus OnUpdate () {
            BaseNode childNode = m_Children[0];
            RunningStatus runningStatus = childNode.Update (agent, blackBoardMemory, deltaTime);
            if (runningStatus != RunningStatus.Executing) {
                return nodeRunningState = RunningStatus.Success;
            } else {
                return nodeRunningState = RunningStatus.Executing;
            }
        }

        protected override void OnReset () {
            m_Children[0].Reset ();
        }
    }
}