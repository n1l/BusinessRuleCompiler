using System.Linq.Expressions;

namespace BusinessRuleCompiler
{
    public interface IConditionExpressionVisitor
    {
        Expression Visit(CompositeCondition condition);

        Expression Visit(BinaryCondition condition);
    }
}
