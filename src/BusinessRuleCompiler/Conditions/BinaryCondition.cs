using System.Linq.Expressions;

namespace BusinessRuleCompiler
{
    public sealed class BinaryCondition : Condition
    {
        public string Fact { get; set; }

        public string Value { get; set; }

        public override Expression Accept(IConditionExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
