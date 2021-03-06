using System;
using Xunit;
using monkey;
using System.Collections.Generic;
using Xunit.Abstractions;

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
        public void TestStringLiteralExpression()
        {
            string input = "\"hello world\"";
            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program program = p.ParseProgram();
            checkParserErrors(p);

            if (program != null) {
                ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                Assert.IsType<StringLiteral>(stmt.expression);
                StringLiteral literal = (StringLiteral)stmt.expression;
                Assert.Equal("hello world", literal.Value);
            }
        }

        [Fact]
        public void TestParsingPrefixExpressions()
        {
            var tests = new List<PrefixTest>{
                new PrefixTest("!5;", "!", 5),
                new PrefixTest("-15", "-", 15),
                new PrefixTest("!True;", "!", true),
                new PrefixTest("!False;", "!", false)
            };

            foreach (PrefixTest tt in tests) {
                Lexer l = new Lexer(tt.input);
                Parser p = new Parser(l);
                Program program = p.ParseProgram();
                checkParserErrors(p);
                Assert.NotNull(program);
                if (program != null) {
                    Assert.Single(program.statements);
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
            Program program = p.ParseProgram();
            checkParserErrors(p);

            Assert.NotNull(program);
            if (program != null) {
                Assert.Single(program.statements);
                Assert.IsType<ExpressionStatement>(program.statements[0]);
                ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                Assert.IsType<IntegerLiteral>(stmt.expression);
                IntegerLiteral literal = (IntegerLiteral)stmt.expression;
                Assert.Equal(5, literal.value);
                Assert.Equal("5", literal.TokenLiteral());
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

        private void testIdentifier(Expression exp, string value)
        {
            Assert.IsType<Identifier>(exp);
            Identifier ident = (Identifier)exp;
            Assert.Equal(ident.value, value);
            Assert.Equal(ident.TokenLiteral(), value);
        }

        private void testBooleanLiteral(Expression expression, bool value)
        {
            Assert.IsType<AstBool>(expression);
            AstBool bo = (AstBool)expression;
            Assert.Equal(bo.value, value);
            Assert.Equal(bo.TokenLiteral(), value.ToString());
        }

        private void testLiteralExpression(Expression exp, object expected)
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


        private void testInfixExpression(Expression exp, object left,
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
            var tests = new List<OperatorPrecedenceTest>{
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
                new OperatorPrecedenceTest("a + add(b * c) + d", "((a + add((b * c))) + d)"),
                new OperatorPrecedenceTest("add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))"),
                new OperatorPrecedenceTest("add(a + b + c * d / f + g)", "add((((a + b) + ((c * d) / f)) + g))")
            
            };

            foreach (var tt in tests) {
                Lexer l = new Lexer(tt.input);
                Parser p = new Parser(l);
                Program program = p.ParseProgram();
                checkParserErrors(p);
                Assert.NotNull(program);
                if (program != null) {
                    string actual = program.String();
                    Assert.Equal(actual, tt.expected);
                }
            }

        }

        class FunctionLiteralParsingCase
        {
            public string input;
            public List<string> expectedParams;
            public FunctionLiteralParsingCase(string i, List<string> e)
            {
                input = i;
                expectedParams = e;
            }
        };

        [Fact]
        public void TestCallExpressionCalling()
        {
            string input = "add(1, 2 * 3, 4 + 5);";
            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program program = p.ParseProgram();
            checkParserErrors(p);
            Assert.NotNull(program);
            if (program != null) {
                Assert.Single(program.statements);
                Assert.IsType<ExpressionStatement>(program.statements[0]);
                ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                Assert.IsType<CallExpression>(stmt.expression);
                CallExpression exp = (CallExpression)stmt.expression;
                testIdentifier(exp.function, "add");
                Assert.Equal(3, exp.arguments.Count);
                testLiteralExpression(exp.arguments[0], 1);
                testInfixExpression(exp.arguments[1], 2, "*", 3);
                testInfixExpression(exp.arguments[2], 4, "+", 5);
            }
        }

        [Fact]
        public void TestFunctionLiteralParsing()
        {
            string input = "fn(x, y) {x + y; }";

            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program program = p.ParseProgram();
            checkParserErrors(p);
            Assert.NotNull(program);
            if (program != null)
            {
                Assert.Single(program.statements);
                Assert.IsType<ExpressionStatement>(program.statements[0]);
                ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                Assert.IsType<FunctionLiteral>(stmt.expression);
                FunctionLiteral function = (FunctionLiteral)stmt.expression;

                Assert.NotNull(function.parameters);
                if (function.parameters != null)
                {
                    Assert.Equal(2, function.parameters.Count);
                    testLiteralExpression(function.parameters[0], "x");
                    testLiteralExpression(function.parameters[1], "y");
                }
                Assert.Single(function.body.statements);
                Assert.IsType<ExpressionStatement>(function.body.statements[0]);
                ExpressionStatement bodyStmt = (ExpressionStatement)function.body.statements[0];
                testInfixExpression(bodyStmt.expression, "x", "+", "y");
            }
        }

        [Fact]
        public void TestFunctionParameterParsing()
        {
            var tests = new List<FunctionLiteralParsingCase>() {
                new FunctionLiteralParsingCase("fn() {};", new List<string>() {}),
                new FunctionLiteralParsingCase("fn(x) {};", new List<string>() {"x"}),
                new FunctionLiteralParsingCase("fn(x, y, z) {};", new List<string>() {"x", "y", "z"})
            };
            
            foreach (var tt in tests) {
                Lexer l = new Lexer(tt.input);
                Parser p = new Parser(l);
                Program program = p.ParseProgram();
                checkParserErrors(p);
                Assert.NotNull(program);
                if (program != null) {
                    Assert.IsType<ExpressionStatement>(program.statements[0]);
                    ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                    Assert.IsType<FunctionLiteral>(stmt.expression);
                    FunctionLiteral function = (FunctionLiteral)stmt.expression;

                    Assert.NotNull(function.parameters);
                    if (function.parameters != null) {
                        Assert.Equal(function.parameters.Count, tt.expectedParams.Count);
                        for (int i = 0; i < tt.expectedParams.Count; ++i) {
                            testLiteralExpression(function.parameters[i], tt.expectedParams[i]);
                        }
                    }
                }
            }
        }

        [Fact]
        public void TestIfExpression()
        {
            string input = "if (x < y) { x }";
            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program program = p.ParseProgram();
            checkParserErrors(p);
            Assert.NotNull(program);
            if (program != null) {
                Assert.Single(program.statements);
                Assert.IsType<ExpressionStatement>(program.statements[0]);
                ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                Assert.IsType<IfExpression>(stmt.expression);
                IfExpression exp = (IfExpression)stmt.expression;
                testInfixExpression(exp.condition, "x", "<", "y");
                Assert.Single(exp.consequence.statements);
                Assert.IsType<ExpressionStatement>(exp.consequence.statements[0]);
                ExpressionStatement consequence = (ExpressionStatement)exp.consequence.statements[0];
                testIdentifier(consequence.expression, "x");
                Assert.Null(exp.alternative);
            }
        }

        [Fact]
        public void TestParsingInfixExpressions()
        {
            var tests = new List<InfixTest> {
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
                Program program = p.ParseProgram();
                checkParserErrors(p);
                Assert.NotNull(program);
                if (program != null) {
                    Assert.Single(program.statements);
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
            Program program = p.ParseProgram();
            checkParserErrors(p);
            Assert.NotNull(program);
            if (program != null) {
                Assert.Single(program.statements);
                Assert.IsType<ExpressionStatement>(program.statements[0]);
                ExpressionStatement stmt = (ExpressionStatement)program.statements[0];
                Assert.IsType<Identifier>(stmt.expression);
                Identifier ident = (Identifier)stmt.expression;
                Assert.Equal("foobar", ident.value);
                Assert.Equal("foobar", ident.TokenLiteral());
            }
        }

        [Fact]
        public void TestDotStatement()
        {
            string input = ".1+2";
            Lexer l = new Lexer(input);
            Parser p = new Parser(l);
            Program program = p.ParseProgram();
            checkParserErrors(p);
            if (program != null) {
                Assert.Single(program.statements);
            }
            if (program != null) {
                foreach (Statement stmt in program.statements) {
                    Assert.IsType<DotStatement>(stmt);
                    DotStatement returnStmt = (DotStatement)stmt;
                    Assert.Equal(".", returnStmt.TokenLiteral());
                }
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
            Program program = p.ParseProgram();
            checkParserErrors(p);
    
            if (program != null) {
                Assert.Equal(3, program.statements.Count);
            }

            if (program != null) {
                foreach (Statement stmt in program.statements) {
                    Assert.IsType<ReturnStatement>(stmt);
                    ReturnStatement returnStmt = (ReturnStatement)stmt;
                    Assert.Equal("return", returnStmt.TokenLiteral());
                }
            }
        }

        public class TestLetStatementCase
        {
            public string input;
            public string expectedIdentifier;
            public object expectedValue;

            public TestLetStatementCase(string i, string e, object ev)
            {
                input = i;
                expectedIdentifier = e;
                expectedValue = ev;
            }
        }
        [Fact]
        public void TestLetStatements()
        {
            List<TestLetStatementCase> tests = new List<TestLetStatementCase>{
                new TestLetStatementCase("let x = 5;", "x", 5),
                new TestLetStatementCase("let y = True;", "y", true),
                new TestLetStatementCase("let foobar = y;", "foobar", "y")
            };

            foreach (TestLetStatementCase tt in tests) {
                Lexer l = new Lexer(tt.input);
                Parser p = new Parser(l);
                
                Program program = p.ParseProgram();
                checkParserErrors(p);
                Assert.NotNull(program);
            
                if (program != null) {
                    Assert.Single(program.statements);
                    Statement stmt = program.statements[0];
                    testLetStatement(stmt, tt.expectedIdentifier);
                    Assert.IsType<LetStatement>(stmt);
                    LetStatement lstmt = (LetStatement)stmt;
                    testLiteralExpression(lstmt.value, tt.expectedValue);
                }
            }
        }
    }
}
