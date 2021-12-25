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

        public RunningStatus update (IAgent agent, BlackBoardMemory workingMemory) {
            this.agent = agent;
            this.blackBoardMemory = workingMemory;

            if (this.m_PreCondition != null && !this.m_PreCondition.isTrue (agent)) {
                return RunningStatus.Failed;
            }

            return onUpdate ();
        }

        public void reset () {
            onReset ();
        }

        public BaseNode addChild (params BaseNode[] children) {
            foreach (BaseNode node in children) {
                node.m_parent = this;
                m_Children.Add (node);
            }

            return this;
        }

        public BaseNode setPreCondition (BaseCondition condition) {
            this.m_PreCondition = condition;
            return this;
        }

        //implemented by inherited class
        protected virtual RunningStatus onUpdate () {
            return RunningStatus.Finished;
        }

        protected virtual void onReset () {

        }
    }
}