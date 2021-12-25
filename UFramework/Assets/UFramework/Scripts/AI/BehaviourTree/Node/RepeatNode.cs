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

        protected override RunningStatus onUpdate () {
            while (true) {
                RunningStatus status = child.update (this.agent, this.blackBoardMemory);
                if (status == RunningStatus.Failed) {
                    return RunningStatus.Failed;
                }

                if (status == RunningStatus.Executing) {
                    break;
                }

                if (++m_repeatIndex == m_repeatCount) {
                    return RunningStatus.Finished;
                }
                child.reset ();
            }

            return RunningStatus.Executing;
        }

        protected override void onReset () {
            child.reset ();
            m_repeatCount = 0;
        }
    }
}