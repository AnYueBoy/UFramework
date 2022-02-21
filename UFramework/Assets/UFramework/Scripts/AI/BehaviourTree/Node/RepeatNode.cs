/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:23:26 
 * @Description: 重复节点
 */
using UnityEngine;

namespace UFramework.AI.BehaviourTree {
    public class RepeatNode : DecoratorNode {

        private int m_repeatCount;
        private int m_repeatIndex;

        public RepeatNode (BaseNode child, int count = 1) : base (child) {
            // base 调用父类的构造函数

            m_repeatCount = Mathf.Max (1, count);
            m_repeatIndex = 0;
        }

        protected override RunningStatus OnUpdate () {
            while (true) {
                RunningStatus status = child.Update (agent, blackBoardMemory, deltaTime);
                if (status == RunningStatus.Failed) {
                    return nodeRunningState = RunningStatus.Failed;
                }

                if (status == RunningStatus.Executing) {
                    break;
                }

                if (++m_repeatIndex == m_repeatCount) {
                    return nodeRunningState = RunningStatus.Success;
                }
                child.Reset ();
            }

            return nodeRunningState = RunningStatus.Executing;
        }

        protected override void OnReset () {
            child.Reset ();
            m_repeatCount = 0;
        }
    }
}