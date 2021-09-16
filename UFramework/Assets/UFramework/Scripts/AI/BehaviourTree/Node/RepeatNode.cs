/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:23:26 
 * @Description: 重复节点
 */
using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Node;
using UFramework.AI.BlackBoard;
using UnityEngine;

namespace UFramework.AI.BehaviourTree.Node {
    public class RepeatNode : DecoratorNode {

        private int m_repeatCount;
        private int m_repeatIndex;

        public RepeatNode (BaseNode child, int count = 1) : base (child) {
            // base 调用父类的构造函数

            m_repeatCount = Mathf.Max (1, count);
            m_repeatIndex = 0;
        }

        protected override RunningStatus onUpdate (IAgent agent, BlackBoardMemory workingMemory) {
            while (true) {
                RunningStatus status = child.update (agent, workingMemory);
                if (status == RunningStatus.Failed) {
                    return RunningStatus.Failed;
                }

                if (status == RunningStatus.Executing) {
                    break;
                }

                if (++m_repeatIndex == m_repeatIndex) {
                    return RunningStatus.Finished;
                }
                child.reset (agent, workingMemory);
            }

            return RunningStatus.Executing;
        }

        protected override void onReset (IAgent agent, BlackBoardMemory workingMemory) {
            child.reset (agent, workingMemory);
            m_repeatCount = 0;
        }
    }
}