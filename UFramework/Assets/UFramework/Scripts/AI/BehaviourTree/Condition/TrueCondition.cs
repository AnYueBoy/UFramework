/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:41:10 
 * @Description: {} 
 */

using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Condition;

public class TrueCondition : BaseCondition {

    public override bool isTrue (IAgent agent) {
        return true;
    }

}