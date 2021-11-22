using System.Linq.Expressions;

namespace BusinessRuleCompiler
{
    public abstract class Condition
    {
        public ExpressionType Operator { get; set; }

        public abstract Expression Accept(IConditionExpressionVisitor visitor);
    }
}
