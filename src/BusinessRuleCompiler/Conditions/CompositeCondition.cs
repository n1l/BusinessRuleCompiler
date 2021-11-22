using System.Collections.Generic;
using System.Linq.Expressions;

namespace BusinessRuleCompiler
{
    public sealed class CompositeCondition : Condition
    {
        public IEnumerable<Condition> Conditions { get; set; }

        public override Expression Accept(IConditionExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
