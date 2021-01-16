/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:41:49 
 * @Description: {} 
 */

using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Condition;

public class FalseCondition : BaseCondition {

    public override bool isTrue (IAgent agent) {
        return false;
    }
}