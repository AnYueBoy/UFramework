/*
 * @Author: l hy 
 * @Date: 2021-01-16 09:16:26 
 * @Description: 基本节点
 */

using System.Collections.Generic;

namespace UFramework.AI.BehaviourTree {
    public class BaseNode {

        // Tree Structure
        protected BaseNode m_parent;
        protected List<BaseNode> m_Children = new List<BaseNode> ();

        // PreCondition
        private BaseCondition m_PreCondition;

        protected IAgent agent;

        protected BlackBoardMemory blackBoardMemory;

        protected RunningStatus nodeRunningState;

        protected float deltaTime;

        public RunningStatus Update (IAgent agent, BlackBoardMemory workingMemory, float deltaTime) {
            this.agent = agent;
            this.blackBoardMemory = workingMemory;
            this.deltaTime = deltaTime;

            if (this.m_PreCondition != null && !this.m_PreCondition.IsTrue (agent)) {
                return nodeRunningState = RunningStatus.Failed;
            }

            return OnUpdate ();
        }

        public void Reset () {
            OnReset ();
        }

        public BaseNode AddChild (params BaseNode[] children) {
            foreach (BaseNode node in children) {
                node.m_parent = this;
                m_Children.Add (node);
            }

            return this;
        }

        public BaseNode SetPreCondition (BaseCondition condition) {
            this.m_PreCondition = condition;
            return this;
        }

        //implemented by inherited class
        protected virtual RunningStatus OnUpdate () {
            return nodeRunningState = RunningStatus.Success;
        }

        protected virtual void OnReset () {

        }
    }
}