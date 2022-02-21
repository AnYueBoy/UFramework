/*
 * @Author: l hy 
 * @Date: 2021-01-16 15:45:39 
 * @Description: 行为树
 */

namespace UFramework.AI.BehaviourTree {
    public class BehaviourTreeRunner {

#if   UNITY_EDITOR
        private BaseNode _root;
#endif

        public void Execute (BaseNode root, IAgent agent, BlackBoardMemory workingMemory, float deltaTime) {
#if UNITY_EDITOR
            this._root = root;
#endif
            RunningStatus status = root.Update (agent, workingMemory, deltaTime);
            if (status != RunningStatus.Executing) {
                root.Reset ();
            }
        }

#if UNITY_EDITOR
        public BaseNode RootNode {
            get {
                return _root;
            }
        }

#endif
    }

}