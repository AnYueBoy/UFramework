/*
 * @Author: l hy 
 * @Date: 2021-01-16 09:16:26 
 * @Description: 基本节点
 */

using System.Collections.Generic;
using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Condition;
using UFramework.AI.BehaviourTree;
using UFramework.AI.BlackBoard;

namespace UFramework.AI.BehaviourTree.Node {
    public class BaseNode {

        // Tree Structure
        protected BaseNode m_parent;
        protected List<BaseNode> m_Children = new List<BaseNode> ();

        // PreCondition
        private BaseCondition m_PreCondition;

        public RunningStatus update (IAgent agent, BlackBoardMemory workingMemory) {
            if (this.m_PreCondition != null && !this.m_PreCondition.isTrue (agent)) {
                return RunningStatus.Failed;
            }

            return onUpdate (agent, workingMemory);
        }

        public void reset (IAgent agent, BlackBoardMemory workingMemory) {
            onReset (agent, workingMemory);
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
        protected virtual RunningStatus onUpdate (IAgent agent, BlackBoardMemory workingMemory) {
            return RunningStatus.Finished;
        }

        protected virtual void onReset (IAgent agent, BlackBoardMemory workingMemory) {

        }
    }
}