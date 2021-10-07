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


        private class TestIfElseExpressionsCase
        {
            public string input { get; set; }
            public object? expected { get; set; }
            public TestIfElseExpressionsCase(string i, object? e)
            {
                input = i;
                expected = e;
            }
        }

        private class TestReturnStatementsCase
        {
            public string input { get; set; }
            public int expected { get; set; }
            public TestReturnStatementsCase(string i, int e)
            {
                input = i;
                expected = e;
            }
        }

        [Fact]
        public void TestReturnStatements()
        {
            var tests = new List<TestReturnStatementsCase>() {
                new TestReturnStatementsCase("return 10;", 10),
                new TestReturnStatementsCase("return 10; 9;", 10),
                new TestReturnStatementsCase("return 2 * 5; 9;", 10),
                new TestReturnStatementsCase("9; return 2 * 5; 9;", 10),
                new TestReturnStatementsCase("if (10 > 1) { if (10 > 1) { return 10; } return 1; }", 10),

            };

            foreach (TestReturnStatementsCase tt in tests) {
                obj.Object evaluated = testEval(tt.input);
                testIntegerObject(evaluated, tt.expected);
            }
        }


        [Fact]
        public void TestIfElseExpressions()
        {
             var tests = new List<TestIfElseExpressionsCase>() {
                new TestIfElseExpressionsCase("if (True) { 10 }", 10),
                new TestIfElseExpressionsCase("if (False) { 10 }", null),
                new TestIfElseExpressionsCase("if (1) { 10 }", 10),
                new TestIfElseExpressionsCase("if (1 < 2) { 10 }", 10),
                new TestIfElseExpressionsCase("if (1 > 2) { 10 }", null),
                new TestIfElseExpressionsCase("if (1 > 2) { 10 } else { 20 }", 20),
                new TestIfElseExpressionsCase("if (1 < 2) { 10 } else { 20 }", 10)
             };
             
            foreach (TestIfElseExpressionsCase tt in tests) {
                obj.Object evaluated = testEval(tt.input);
                if (tt.expected == null) {
                    testNullObject(evaluated);
                } else {
                    Type t = tt.expected.GetType();
                    if (t == typeof(int)) {
                        testIntegerObject(evaluated, (int)tt.expected);
                    }
                }
            }
        }

        private void testNullObject(obj.Object obj)
        {
            Assert.Equal(obj, Evaluator.NULL);
        }

        [Fact]
        public void TestEvalBooleanExpression()
        {
            var tests = new List<TestEvalBooleanExpressionCase>() {
                new TestEvalBooleanExpressionCase("True", true),
                new TestEvalBooleanExpressionCase("False", false),
                new TestEvalBooleanExpressionCase("1 < 2", true),
                new TestEvalBooleanExpressionCase("1 > 2", false),
                new TestEvalBooleanExpressionCase("1 < 1", false),
                new TestEvalBooleanExpressionCase("1 > 1", false),
                new TestEvalBooleanExpressionCase("1 == 1", true),
                new TestEvalBooleanExpressionCase("1 != 1", false),
                new TestEvalBooleanExpressionCase("1 == 2", false),
                new TestEvalBooleanExpressionCase("1 != 2", true),
                new TestEvalBooleanExpressionCase("True == True", true),
                new TestEvalBooleanExpressionCase("False == False", true),
                new TestEvalBooleanExpressionCase("True == False", false),
                new TestEvalBooleanExpressionCase("True != False", true),
                new TestEvalBooleanExpressionCase("False != True", true),
                new TestEvalBooleanExpressionCase("(1 < 2) == True", true),
                new TestEvalBooleanExpressionCase("(1 < 2) == False", false),
                new TestEvalBooleanExpressionCase("(1 > 2) == True", false),
                new TestEvalBooleanExpressionCase("(1 > 2) == False", true),
            };

            foreach (TestEvalBooleanExpressionCase tt in tests) {
                obj.Object evaluated = testEval(tt.input);
                testBooleanObject(evaluated, tt.expected);
            }
        }
        
        private class TestBangOperatorCase
        {
            public string input { get; set; }
            public bool expected { get; set; }
            public TestBangOperatorCase(string i, bool e)
            {
                input = i;
                expected = e;
            }
        }

        [Fact]
        public void TestBangOperator()
        {
            var tests = new List<TestBangOperatorCase>() {
                new TestBangOperatorCase("!True", false),
                new TestBangOperatorCase("!False", true),
                new TestBangOperatorCase("!5", false),
                new TestBangOperatorCase("!!True", true),
                new TestBangOperatorCase("!!False", false),
                new TestBangOperatorCase("!!5", true)
            };

            foreach (TestBangOperatorCase tt in tests) {
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
                new TestEvalIntegerExpressionCase("-5", -5),
                new TestEvalIntegerExpressionCase("-10", -10),
                new TestEvalIntegerExpressionCase("5 + 5 + 5 + 5 - 10", 10),
                new TestEvalIntegerExpressionCase("2 * 2 * 2 * 2 * 2", 32),
                new TestEvalIntegerExpressionCase("-50 + 100 + -50", 0),
                new TestEvalIntegerExpressionCase("5 * 2 + 10", 20),
                new TestEvalIntegerExpressionCase("5 + 2 * 10", 25),
                new TestEvalIntegerExpressionCase("20 + 2 * -10", 0),
                new TestEvalIntegerExpressionCase("50 / 2 * 2 + 10", 60),
                new TestEvalIntegerExpressionCase("2 * (5 + 10)", 30),
                new TestEvalIntegerExpressionCase("3 * 3 * 3 + 10", 37),
                new TestEvalIntegerExpressionCase("3 * (3 * 3) + 10", 37),
                new TestEvalIntegerExpressionCase("(5 + 10 * 2 + 15 / 3) * 2 + -10", 50),
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
