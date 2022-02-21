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

        protected override RunningStatus OnUpdate () {
            if (m_Children.Count == 0) {
                return nodeRunningState = RunningStatus.Success;
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
                    status = m_Children[i].Update (agent, blackBoardMemory, deltaTime);
                }

                if (status == RunningStatus.Success) {
                    finishedCount++;
                    m_childrenRunning[i] = status;
                    if (finishedCount == m_requestFinishedCount) {
                        return nodeRunningState = RunningStatus.Success;
                    }
                } else if (status == RunningStatus.Failed) {
                    failedCount++;
                    if (failedCount > m_Children.Count - m_requestFinishedCount) {
                        return nodeRunningState = RunningStatus.Failed;
                    }
                }
            }

            return nodeRunningState = RunningStatus.Executing;
        }

        protected override void OnReset () {
            foreach (BaseNode node in m_Children) {
                node.Reset ();
            }

            m_childrenRunning.Clear ();
        }
    }
}