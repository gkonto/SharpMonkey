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

        private class TestEvalBooleanExpressionCase
        {
            public string input { get; set; }
            public bool expected { get; set; }
            public TestEvalBooleanExpressionCase(string i, bool e)
            {
                input = i;
                expected = e;
            }
        }

        [Fact]
        public void TestEvalBooleanExpression()
        {
            var tests = new List<TestEvalBooleanExpressionCase>() {
                new TestEvalBooleanExpressionCase("true", true),
                new TestEvalBooleanExpressionCase("false", false)
            };

            foreach (TestEvalBooleanExpressionCase tt in tests) {
                obj.Object evaluated = testEval(tt.input);
                testBooleanObject(evaluated, tt.expected);
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

        private void testBooleanObject(obj.Object obj, bool expected)
        {
            Assert.IsType<obj.Boolean>(obj);
            obj.Boolean result = (obj.Boolean)obj;
            Assert.Equal(result.Value, expected);
        }
    }
}
