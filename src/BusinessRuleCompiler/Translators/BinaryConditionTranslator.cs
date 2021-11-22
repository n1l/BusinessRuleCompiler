using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BusinessRuleCompiler.Translators
{
    internal partial class ConditionTranslator
    {
        private class BinaryConditionTranslator
        {
            private readonly Type _modelType;
            private readonly ParameterExpression _parameter;

            internal BinaryConditionTranslator(Type modelType, ParameterExpression parameter)
            {
                _modelType = modelType;
                _parameter = parameter;
            }

            internal Expression Translate(BinaryCondition condition)
            {
                if (string.IsNullOrWhiteSpace(condition.Fact))
                {
                    throw new ArgumentException($"'{nameof(BinaryCondition)}.{nameof(condition.Fact)}' can not be null or empty.");
                }

                if (condition.Value == null)
                {
                    throw new ArgumentException($"'{nameof(BinaryCondition)}.{nameof(condition.Value)}' can not be null.");
                }

                if (!FactIsPresented())
                {
                    throw new ArgumentException($"Fact '{condition.Fact}' is not available.");
                }

                return TranslateToBinary();
                
                bool FactIsPresented() => _modelType.GetProperty(condition.Fact) != null;

                Expression TranslateToBinary()
                {
                    try
                    {
                        PropertyInfo property = _modelType.GetProperty(condition.Fact);

                        MemberExpression left = Expression.Property(_parameter, condition.Fact);

                        Expression right = property.PropertyType.GetGenericArguments().Any()
                            ? (Expression)Expression.Convert(Expression.Constant(
                                Convert.ChangeType(
                                    condition.Value,
                                    property.PropertyType.GetGenericArguments().First(),
                                    CultureInfo.InvariantCulture)),
                                property.PropertyType)
                            : Expression.Constant(
                                Convert.ChangeType(
                                    condition.Value,
                                    property.PropertyType,
                                    CultureInfo.InvariantCulture));

                        return Expression.MakeBinary(condition.Operator, left, right);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Rule compilation exception", ex);
                    }
                }
            }
        }
    }
}
