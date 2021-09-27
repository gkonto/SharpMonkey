using System;
using Xunit;
using lexer;
using parser;
using ast;
using System.Collections.Generic;
using Xunit.Abstractions;

#nullable enable

namespace ParserTest
{
    public class ParserTests
    {
        
        private readonly ITestOutputHelper output;
        public ParserTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private void testLetStatement(Statement s, string name)
        {
            Assert.Equal("let", s.TokenLiteral());
            Assert.IsType<LetStatement>(s);
            LetStatement ls = (LetStatement)s;
            Assert.Equal(ls.name.value, name);
            Assert.Equal(ls.name.TokenLiteral(), name);

        }

        private void checkParserErrors(Parser p)
        {
            var errors = p.errors;
            if (errors.Count == 0) {
                return;
            }
            output.WriteLine($"parser has {errors.Count} errors.");
            for (int i = 0; i < errors.Count; ++i) {
                output.WriteLine($"parser error {errors[i]}");
            }
            Assert.True(true);
        }

        public class PrefixTest
        {
            public string input;
            public string op;
            public object value;

            public PrefixTest(string i, string o, object integer)
            {
                input = i;
                op = o;
                value = integer;
            }
        }

        private void testIntegerLiteral(Expression il, int value)
        {
            Assert.IsType<IntegerLiteral>(il);
            IntegerLiteral integ = (IntegerLiteral)il;
            Assert.Equal(integ.value, value);
            Assert.Equal(integ.TokenLiteral(), value.ToString());
        }

        [Fact]
        public void TestParsingPrefixExpressions()
        {
            var tests = new List<PrefixTest>() {
                new PrefixTest("!5;", "!", 5),
                new PrefixTest("-15", "-", 15),
                new PrefixTest("!True;", "!", true),
                new PrefixTest("!False;", "!", false)
            };

            foreach (PrefixTest tt in tests) {
                Lexer l = new Lexer(tt.input);
                Parser p = new Parser(l);
                Program? program = p.ParseProgram();
                checkParserErrors(p);
                Assert.NotNull(program);
                if (program != null) {
                    Assert.Equal(program.statements.Count, 1);
                    Assert.IsType<ExpressionStatement>(program.statements[0]);
                    ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                    Assert.IsType<PrefixExpression>(stmt.expression);
                    PrefixExpression exp = (PrefixExpression)stmt.expression;
                    Assert.Equal(exp.Operator, tt.op);
                    testLiteralExpression(exp.right, tt.value);
                }
            }
        }

        [Fact]
        public void TestIntegerLiteralExpression()
        {
            string input = "5;";
            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program? program = p.ParseProgram();
            checkParserErrors(p);

            Assert.NotNull(program);
            if (program != null) {
                Assert.Equal(program.statements.Count, 1);
                Assert.IsType<ExpressionStatement>(program.statements[0]);
                ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                Assert.IsType<IntegerLiteral>(stmt.expression);
                IntegerLiteral literal = (IntegerLiteral)stmt.expression;
                Assert.Equal(literal.value, 5);
                Assert.Equal(literal.TokenLiteral(), "5");
            }
        }


        public class InfixTest
        {
            public string input;
            public object leftValue;
            public string Operator;
            public object rightValue;

            public InfixTest(string i, object lvalue, string o, object rvalue)
            {
                input = i;
                leftValue = lvalue;
                Operator = o;
                rightValue = rvalue;
            }
        }

        public class OperatorPrecedenceTest
        {
            public OperatorPrecedenceTest(string i, string e)
            {
                input = i;
                expected = e;
            }

            public string input;
            public string expected;
        }

        public void testIdentifier(Expression exp, string value)
        {
            Assert.IsType<Identifier>(exp);
            Identifier ident = (Identifier)exp;
            Assert.Equal(ident.value, value);
            Assert.Equal(ident.TokenLiteral(), value);
        }

        private void testBooleanLiteral(Expression expression, bool value)
        {
            Assert.IsType<ast.Boolean>(expression);
            ast.Boolean bo = (ast.Boolean)expression;
            Assert.Equal(bo.value, value);
            Assert.Equal(bo.TokenLiteral(), value.ToString());
        }

        public void testLiteralExpression(Expression exp, object expected)
        {
            Type t = expected.GetType();
            if (t.Equals(typeof(int))) {
                testIntegerLiteral(exp, (int)expected);
            } else if (t.Equals(typeof(string))) {
                testIdentifier(exp, (string)expected);
            } else if (t.Equals(typeof(bool))) {
                testBooleanLiteral(exp, (bool)expected);
            }
            Assert.True(true, $"type of exp not handled. Got {exp}");
        }


        public void testInfixExpression(Expression exp, object left,
                                        string op, object right)
        {
            Assert.IsType<InfixExpression>(exp);
            InfixExpression opExp = (InfixExpression)exp;
            testLiteralExpression(opExp.left, left);
            Assert.Equal(opExp.Operator, op);
            testLiteralExpression(opExp.right, right);
        }

        [Fact]
        public void TestOperatorPrecedenceParsing()
        {
            var tests = new List<OperatorPrecedenceTest>() {
                new OperatorPrecedenceTest("-a * b", "((-a) * b)"),
                new OperatorPrecedenceTest("!-a", "(!(-a))"),
                new OperatorPrecedenceTest("a + b + c", "((a + b) + c)"),
                new OperatorPrecedenceTest("a + b - c", "((a + b) - c)"),
                new OperatorPrecedenceTest("a * b * c", "((a * b) * c)"),
                new OperatorPrecedenceTest("a * b / c", "((a * b) / c)"),
                new OperatorPrecedenceTest("a + b / c", "(a + (b / c))"),
                new OperatorPrecedenceTest("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"),
                new OperatorPrecedenceTest("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)"),
                new OperatorPrecedenceTest("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"),
                new OperatorPrecedenceTest("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))"),
                new OperatorPrecedenceTest("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
                new OperatorPrecedenceTest("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),          
                new OperatorPrecedenceTest("true", "true"),
                new OperatorPrecedenceTest("false", "false"),
                new OperatorPrecedenceTest("3 > 5 == false", "((3 > 5) == false)"),
                new OperatorPrecedenceTest("3 < 5 == true","((3 < 5) == true)"),
                new OperatorPrecedenceTest("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"),
                new OperatorPrecedenceTest("(5 + 5) * 2", "((5 + 5) * 2)"),
                new OperatorPrecedenceTest("2 / (5 + 5)", "(2 / (5 + 5))"),
                new OperatorPrecedenceTest("-(5 + 5)", "(-(5 + 5))"),
                new OperatorPrecedenceTest("!(true == true)", "(!(true == true))"),
            };

            foreach (var tt in tests) {
                Lexer l = new Lexer(tt.input);
                Parser p = new Parser(l);
                Program? program = p.ParseProgram();
                checkParserErrors(p);
                Assert.NotNull(program);
                if (program != null) {
                    string actual = program.String();
                    Assert.Equal(actual, tt.expected);
                }
            }

        }

        [Fact]
        public void TestParsingInfixExpressions()
        {
            var tests = new List<InfixTest>() {
                new InfixTest("5 + 5;", 5, "+", 5),
                new InfixTest("5 - 5;", 5, "-", 5),
                new InfixTest("5 * 5;", 5, "*", 5),
                new InfixTest("5 / 5;", 5, "/", 5),
                new InfixTest("5 > 5;", 5, ">", 5),
                new InfixTest("5 < 5;", 5, "<", 5),
                new InfixTest("5 == 5;", 5, "==", 5),
                new InfixTest("5 != 5;", 5, "!=", 5),
                new InfixTest("True == True", true, "==", true),
                new InfixTest("True != False", true, "!=", false),
                new InfixTest("False == False", false, "==", false)
            };

            foreach (InfixTest tt in tests) {
                Lexer l = new Lexer(tt.input);
                Parser p = new Parser(l);
                Program? program = p.ParseProgram();
                checkParserErrors(p);
                Assert.NotNull(program);
                if (program != null) {
                    Assert.Equal(program.statements.Count, 1);
                    Assert.IsType<ExpressionStatement>(program.statements[0]);
                    ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                    Assert.IsType<InfixExpression>(stmt.expression);
                    InfixExpression exp = (InfixExpression)stmt.expression;

                    testLiteralExpression(exp.left, tt.leftValue);
                    Assert.Equal(exp.Operator, tt.Operator);
                    testLiteralExpression(exp.right, tt.rightValue);
                }
            }

        }


        [Fact]
        public void TestIdentifierExpression()
        {
            string input = "foobar;";
            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program? program = p.ParseProgram();
            checkParserErrors(p);
            Assert.NotNull(program);
            if (program != null) {
                Assert.Equal(program.statements.Count, 1);
                Assert.IsType<ExpressionStatement>(program.statements[0]);
                ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                Assert.IsType<Identifier>(stmt.expression);
                Identifier ident = (Identifier)stmt.expression;
                Assert.Equal(ident.value, "foobar");
                Assert.Equal(ident.TokenLiteral(), "foobar");
            }
        }

        [Fact]
        public void TestReturnStatements()
        {
            string input = @"return 5;
                            return 10;
                            return 993322;";
            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program? program = p.ParseProgram();
            checkParserErrors(p);
    
            if (program != null) {
                Assert.Equal(3, program.statements.Count);
            }

            if (program != null) {
                foreach (Statement stmt in program.statements) {
                    Assert.IsType<ReturnStatement>(stmt);
                    ReturnStatement returnStmt = (ReturnStatement)stmt;
                    Assert.Equal(returnStmt.TokenLiteral(), "return");
                }
            }
        }

        [Fact]
        public void TestLetStatements()
        {
            string input = @"let x = 5;
                            let y = 10;
                            let foobar = 838383;";

            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            
            Program? program = p.ParseProgram();
            checkParserErrors(p);
            
            Assert.NotNull(program);
            
            if (program != null) {
                Assert.Equal(3, program.statements.Count);
            }
            
            var tests = new List<string>() { "x", "y", "foobar" };

            for (int i = 0; i < tests.Count; ++i) {
                string tt = tests[i];
                if (program != null) {
                    var stmt = program.statements[i];
                    testLetStatement(stmt, tt);
                }
            }
            /*
            */
        }
    }
}
