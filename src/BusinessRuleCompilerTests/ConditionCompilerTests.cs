using BusinessRuleCompiler;
using System;
using System.Linq.Expressions;
using Xunit;

namespace BusinessRuleCompilerTests
{
    public class ConditionCompilerTests
    {
        private readonly ConditionCompiler<PriceModel> _priceRuleCompiler
            = new ConditionCompiler<PriceModel>();

        [Theory]
        [InlineData("LME-AL", 42, true)]
        [InlineData("CME-AL", 42, false)]
        [InlineData("LME", 12, false)]
        public void GivenARule_WhenCompiledAndInvoked_ThenReturnsCorrectResult(string symbol, decimal ask, bool expected)
        {
            var incomingPriceUpdate = new PriceModel { Symbol = symbol, Ask = ask };
            var condition = new CompositeCondition
            {
                Operator = ExpressionType.And,
                Conditions = new Condition[]
                    {
                        new BinaryCondition
                        {
                            Operator = ExpressionType.Equal,
                            Fact = "Symbol",
                            Value = "LME-AL"
                        },
                        new BinaryCondition
                        {
                            Operator = ExpressionType.GreaterThan,
                            Fact = "Ask",
                            Value = "20"
                        },
                    }
            };

            Func<PriceModel, bool> rule = _priceRuleCompiler.Compile(condition);
            var invocationResult = rule(incomingPriceUpdate);

            Assert.Equal(expected, invocationResult);
        }

        [Theory]
        [InlineData("LME-CO", 12, true)]
        [InlineData("LME-GO", 12, true)]
        [InlineData("LME-AL", 12, false)]
        [InlineData("LME-CO", 42, false)]
        public void GivenARuleTracksMultipleSymbols_WhenCompiledAndInvoked_ThenReturnsCorrectResult(string symbol, decimal mid, bool expected)
        {
            var incomingPriceUpdate = new PriceModel { Symbol = symbol, Mid = mid };
            var condition = new CompositeCondition()
            {
                Operator = ExpressionType.And,
                Conditions = new Condition[]
                    {
                        new CompositeCondition
                        {
                            Operator = ExpressionType.Or,
                            Conditions = new Condition[]
                            {
                                new BinaryCondition
                                {
                                    Operator = ExpressionType.Equal,
                                    Fact = "Symbol",
                                    Value = "LME-CO"
                                },
                                new BinaryCondition
                                {
                                    Operator = ExpressionType.Equal,
                                    Fact = "Symbol",
                                    Value = "LME-GO"
                                },
                            }
                        },
                        new BinaryCondition
                        {
                            Operator = ExpressionType.LessThan,
                            Fact = "Mid",
                            Value = "14"
                        }
                    }
            };

            Func<PriceModel, bool> rule = _priceRuleCompiler.Compile(condition);
            var invocationResult = rule(incomingPriceUpdate);

            Assert.Equal(expected, invocationResult);
        }

        [Fact]
        public void GivenInvalidRuleDefinition_WhenCompiled_ThenThrows()
        {
            var condition = new CompositeCondition()
            {
                Operator = ExpressionType.And,
                Conditions = new Condition[]
                    {
                        new BinaryCondition
                        {
                            Operator = ExpressionType.LessThan,
                            Fact = "Symbol",
                            Value = "33"
                        },
                    }
            };

            Assert.Throws<InvalidOperationException>(() => _priceRuleCompiler.Compile(condition));
        }
    }
}
