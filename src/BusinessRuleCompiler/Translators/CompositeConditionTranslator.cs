using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessRuleCompiler.Translators
{
    internal partial class ConditionTranslator
    {
        private class CompositeConditionTranslator
        {
            private readonly ConditionTranslator _parent;

            internal CompositeConditionTranslator(ConditionTranslator translator)
            {
                _parent = translator;
            }

            internal Expression Translate(CompositeCondition condition)
            {
                List<Exception> exceptions = new List<Exception>();
                Expression[] translated = condition.Conditions.Select(child => Translate(child)).ToArray();

                if (exceptions.Any())
                {
                    throw new InvalidOperationException("Rule compilation exception", new AggregateException(exceptions));
                }

                return translated.Aggregate((left, right) => Expression.MakeBinary(condition.Operator, left, right));

                Expression Translate(Condition condition)
                {
                    try
                    {
                        return _parent.Translate(condition);
                    }
                    catch (AggregateException ex)
                    {
                        foreach (Exception innerEx in ex.InnerExceptions)
                        {
                            exceptions.Add(innerEx);
                        }

                        return Expression.Empty();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);

                        return Expression.Empty();
                    }
                }
            }
        }
    }
}
