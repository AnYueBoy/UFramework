/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:45:39 
 * @Description: 行为树
 */
using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Node;
using UFramework.AI.BlackBoard;

namespace UFramework.AI.BehaviourTree {
    public static class BehaviourTreeRunner {

        public static void execute (BaseNode root, IAgent agent, BlackBoardMemory workingMemory) {

            RunningStatus status = root.update (agent, workingMemory);
            if (status != RunningStatus.Executing) {
                root.reset (agent, workingMemory);
            }
        }
    }
}