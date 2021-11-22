using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace BusinessRuleCompiler
{
    class Program
    {
        private static readonly ConditionCompiler<PriceModel> _priceRuleCompiler
            = new ConditionCompiler<PriceModel>();

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }

            int numRules = Int32.Parse(args[0]);
            int facts = Int32.Parse(args[1]);
            var fact = new PriceModel { Symbol = "LME-GO", Mid = 7 };
            
            var sw = new Stopwatch();
            Func<PriceModel, bool>[] rules = new Func<PriceModel, bool>[numRules + 1];
            sw.Start();
            for (int i = 0; i <= numRules; i++)
            {
                rules[i] = CreateRule();
            }
            sw.Stop();
            Console.WriteLine("{0} ms to compile {1} rules, average {2} ms per rule",
                sw.ElapsedMilliseconds,
                numRules,
                ((double)sw.ElapsedMilliseconds / numRules));

            sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i <= facts; i++)
            {
                foreach (var rule in rules)
                {
                    rule(fact);
                }
            }
            sw.Stop();
            Console.WriteLine("{0} ms to evaluate {1} rules against {2} facts, average {3} ms per rule",
                sw.ElapsedMilliseconds,
                numRules,
                facts,
                ((double)sw.ElapsedMilliseconds / numRules));
        }

        private static Func<PriceModel, bool> CreateRule()
        {
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

            return _priceRuleCompiler.Compile(condition);
        }
    }

    public class PriceModel
    {
        public virtual string Symbol { get; set; }

        public virtual decimal Ask { get; set; }

        public virtual decimal Bid { get; set; }

        public virtual decimal Mid { get; set; }
    }
}
