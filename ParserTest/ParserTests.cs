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
            public int integerValue;

            public PrefixTest(string i, string o, int integer)
            {
                input = i;
                op = o;
                integerValue = integer;
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

                    testIntegerLiteral(exp.right, tt.integerValue);
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
