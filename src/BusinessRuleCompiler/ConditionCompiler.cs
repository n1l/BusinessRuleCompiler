using BusinessRuleCompiler.Translators;
using System;
using System.Linq.Expressions;

namespace BusinessRuleCompiler
{
    public class ConditionCompiler<TModel> : IConditionCompiler<TModel>
    {
        private readonly Type _modelType;
        private readonly ParameterExpression _parameter;
        private readonly ConditionTranslator _translator;

        public ConditionCompiler()
        {
            _modelType = typeof(TModel);
            _parameter = Expression.Parameter(_modelType);

            _translator = new ConditionTranslator(_modelType, _parameter);
        }

        public Func<TModel, bool> Compile(Condition condition)
            => Expression.Lambda<Func<TModel, bool>>(
                    _translator.Translate(condition),
                    _parameter)
                .Compile();
    }
}
