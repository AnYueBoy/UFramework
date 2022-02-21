/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:45:39 
 * @Description: 行为树
 */

namespace UFramework.AI.BehaviourTree {
    public static class BehaviourTreeRunner {

        public static void Execute (BaseNode root, IAgent agent, BlackBoardMemory workingMemory, float deltaTime) {
            RunningStatus status = root.Update (agent, workingMemory, deltaTime);
            if (status != RunningStatus.Executing) {
                root.Reset ();
            }
        }
    }
}