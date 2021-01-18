/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:43:42 
 * @Description: 或条件 
 */
using UFramework.AI.BehaviourTree.Agent;
using UFramework.AI.BehaviourTree.Condition;

public class OrCondition : BaseCondition {

    private BaseCondition m_LHS;
    private BaseCondition m_RHS;

    public OrCondition (BaseCondition lhs, BaseCondition rhs) {
        this.m_LHS = lhs;
        this.m_RHS = rhs;
    }

    public override bool isTrue (IAgent agent) {
        return this.m_LHS.isTrue (agent) || this.m_RHS.isTrue (agent);
    }
}