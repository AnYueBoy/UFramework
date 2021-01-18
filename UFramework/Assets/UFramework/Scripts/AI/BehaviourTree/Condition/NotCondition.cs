/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:57:05 
 * @Description: 非条件
 */

using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Condition;

public class NotCondition : BaseCondition {

    private BaseCondition m_LHS;

    public NotCondition (BaseCondition lhs) {
        this.m_LHS = lhs;
    }

    public override bool isTrue (IAgent agent) {
        return !this.m_LHS.isTrue (agent);
    }
}