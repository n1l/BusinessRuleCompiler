using System;

namespace BusinessRuleCompiler
{
    public interface IConditionCompiler<TModel>
    {
        Func<TModel, bool> Compile(Condition condition);
    }
}
