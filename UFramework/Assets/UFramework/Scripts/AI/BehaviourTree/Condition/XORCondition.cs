/*
 * @Author: l hy 
 * @Date: 2021-01-16 13:54:13 
 * @Description: 异或条件
 */

namespace UFramework.AI.BehaviourTree {
    public class XORCondition : BaseCondition {

        private BaseCondition m_LHS;
        private BaseCondition m_RHS;

        public XORCondition (BaseCondition lhs, BaseCondition rhs) {
            this.m_LHS = lhs;
            this.m_RHS = rhs;
        }

        public override bool isTrue (IAgent agent) {
            return this.m_LHS.isTrue (agent) ^ this.m_RHS.isTrue (agent);
        }
    }
}