/*
 * @Author: l hy 
 * @Date: 2021-01-16 14:59:57 
 * @Description: 并行节点（控制流节点）
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UFramework.AI.BehaviourTree {
    public class ParallelNode : BaseNode {

        private int m_requestFinishedCount;
        private List<RunningStatus> m_childrenRunning = new List<RunningStatus> ();
        public ParallelNode (int threshold) {
            // 设定阈值
            m_requestFinishedCount = threshold;
        }

        protected override RunningStatus onUpdate () {
            if (m_Children.Count == 0) {
                return RunningStatus.Finished;
            }

            m_requestFinishedCount = Mathf.Clamp (m_requestFinishedCount, 1, m_Children.Count);
            if (m_childrenRunning.Count != m_Children.Count) {
                m_childrenRunning.AddRange (Enumerable.Repeat (RunningStatus.Executing, m_Children.Count));
            }

            int failedCount = 0;
            int finishedCount = 0;
            for (int i = 0; i < m_Children.Count; ++i) {
                RunningStatus status = m_childrenRunning[i];
                if (status == RunningStatus.Executing) {
                    status = m_Children[i].update (this.agent, this.blackBoardMemory);
                }

                if (status == RunningStatus.Finished) {
                    finishedCount++;
                    m_childrenRunning[i] = status;
                    if (finishedCount == m_requestFinishedCount) {
                        return RunningStatus.Finished;
                    }
                } else if (status == RunningStatus.Failed) {
                    failedCount++;
                    if (failedCount > m_Children.Count - m_requestFinishedCount) {
                        return RunningStatus.Failed;
                    }
                }
            }

            return RunningStatus.Executing;
        }

        protected override void onReset () {
            foreach (BaseNode node in m_Children) {
                node.reset ();
            }

            m_childrenRunning.Clear ();
        }
    }
}