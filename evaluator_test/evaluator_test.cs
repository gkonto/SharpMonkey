using System;
using Xunit;
using System.Collections.Generic;
using lexer;
using parser;
using ast;
using evaluator;
using obj;

#nullable enable

namespace evaluator_test
{
    public class EvaluatorTest
    {
        private class TestEvalIntegerExpressionCase
        {
            public string input { get; set; }
            public int expected { get; set; }
            public TestEvalIntegerExpressionCase(string i, int e)
            {
                input = i;
                expected = e;
            }
        }

        [Fact]
        public void TestEvalIntegerExpression()
        {
            var tests = new List<TestEvalIntegerExpressionCase>() {
                new TestEvalIntegerExpressionCase("5", 5),
                new TestEvalIntegerExpressionCase("10", 10),
            };

            foreach (TestEvalIntegerExpressionCase tt in tests) {
                obj.Object evaluated = testEval(tt.input);
                testIntegerObject(evaluated, tt.expected);
            }
        }

        private obj.Object testEval(string input)
        {
            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program? program = p.ParseProgram();
            return Evaluator.Eval(program);
        }

        private void testIntegerObject(obj.Object obj, int expected)
        {
            Assert.IsType<Integer>(obj);
            Integer result = (Integer)obj;
            Assert.Equal(result.Value, expected);
        }
    }
}
