using System;
using System.Linq.Expressions;

namespace BusinessRuleCompiler.Translators
{
    internal partial class ConditionTranslator : IConditionExpressionVisitor
    {
        private readonly BinaryConditionTranslator _binaryTranslator;
        private readonly CompositeConditionTranslator _compositeTranslator;

        internal ConditionTranslator(Type modelType, ParameterExpression parameter)
        {
            _binaryTranslator = new BinaryConditionTranslator(modelType, parameter);
            _compositeTranslator = new CompositeConditionTranslator(this);
        }

        internal Expression Translate(Condition condition)
        {
            return condition.Accept(this);
        }

        Expression IConditionExpressionVisitor.Visit(CompositeCondition condition)
            => _compositeTranslator.Translate(condition);

        Expression IConditionExpressionVisitor.Visit(BinaryCondition condition)
            => _binaryTranslator.Translate(condition);
    }
}
