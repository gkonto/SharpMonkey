using System;
using Xunit;
using lexer;
using static System.Console;
using Xunit.Abstractions;
using System.Collections.Generic;
using token;
using static token.TokenType;

namespace lexer_test
{
    public class LexerTests
    {
        private readonly ITestOutputHelper output;
        public LexerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestNextToken5()
        {
            string input = @"let five = 5;
                            let ten = 10;
                            let add = fn(x, y) {
                                x + y;
                            };
                            let result = add(five, ten);
                            !-/*5;
                            5 < 10 > 5;
                            if (5 < 10) {
                                return true;
                            } else {
                                return false;
                            }
                            
                            10 == 10;
                            10 != 9;
                            ";
            var tests = new List<Token>() {
            };

            TestCore(input, tests);
        }

        [Fact]
        public void TestNextToken4()
        {
            string input = "let";
            var tests = new List<Token>() {
                new Token() {Type = LET, Literal = "let"},
            };

            TestCore(input, tests);
        }

        [Fact]
        public void TextNextToken3()
        {
            string input = "let five";
            var tests = new List<Token>() {
                new Token() {Type = LET, Literal = "let"},
                new Token() {Type = IDENT, Literal = "five"},
            };

            TestCore(input, tests);
        }

        [Fact]
        public void TestNextToken2()
        {
            string input = "let five = 5;";
            var tests = new List<Token>() {
                new Token() {Type = LET, Literal = "let"},
                new Token() {Type = IDENT, Literal = "five"},
                new Token() {Type = ASSIGN, Literal = "="},
                new Token() {Type = INT, Literal = "5"},
                new Token() {Type = SEMICOLON, Literal = ";"},
            };

            TestCore(input, tests);
        }

        [Fact]
        public void TestNextToken1()
        {
            string input = @"let five = 5;
                            let ten = 10;
                            let add = fn(x, y) {
                                x + y;
                            };
                            let result = add(five, ten);";

            var tests = new List<Token>() {
                new Token() {Type = LET, Literal = "let"},
                new Token() {Type = IDENT, Literal = "five"},
                new Token() {Type = ASSIGN, Literal = "="},
                new Token() {Type = INT, Literal = "5"},
                new Token() {Type = SEMICOLON, Literal = ";"},
                new Token() {Type = LET, Literal = "let"},
                new Token() {Type = IDENT, Literal = "ten"},
                new Token() {Type = ASSIGN, Literal = "="},
                new Token() {Type = INT, Literal = "10"},
                new Token() {Type = SEMICOLON, Literal = ";"},
                new Token() {Type = LET, Literal = "let"},
                new Token() {Type = IDENT, Literal = "add"},
                new Token() {Type = ASSIGN, Literal = "="},
                new Token() {Type = FUNCTION, Literal = "fn"},
                new Token() {Type = LPAREN, Literal = "("},
                new Token() {Type = IDENT, Literal = "x"},
                new Token() {Type = COMMA, Literal = ","},
                new Token() {Type = IDENT, Literal = "y"},
                new Token() {Type = RPAREN, Literal = ")"},
                new Token() {Type = LBRACE, Literal = "{"},
                new Token() {Type = IDENT, Literal = "x"},
                new Token() {Type = PLUS, Literal = "+"},
                new Token() {Type = IDENT, Literal = "y"},
                new Token() {Type = SEMICOLON, Literal = ";"},
                new Token() {Type = RBRACE, Literal = "}"},
                new Token() {Type = SEMICOLON, Literal = ";"},
                new Token() {Type = LET, Literal = "let"},
                new Token() {Type = IDENT, Literal = "result"},
                new Token() {Type = ASSIGN, Literal = "="},
                new Token() {Type = IDENT, Literal = "add"},
                new Token() {Type = LPAREN, Literal = "("},
                new Token() {Type = IDENT, Literal = "five"},
                new Token() {Type = COMMA, Literal = ","},
                new Token() {Type = IDENT, Literal = "ten"},
                new Token() {Type = RPAREN, Literal = ")"},
                new Token() {Type = SEMICOLON, Literal = ";"},
                new Token() {Type = EOF, Literal = ""},
            };

            TestCore(input, tests);
        }


        [Fact]
        public void TestNextToken()
        {
            string input = "=+(){},;";
           
            var tests = new List<Token>() {
                new Token() {Type = ASSIGN, Literal = "="},
                new Token() {Type = PLUS, Literal = "+"},
                new Token() {Type = LPAREN, Literal = "("},
                new Token() {Type = RPAREN, Literal = ")"},
                new Token() {Type = LBRACE, Literal = "{"},
                new Token() {Type = RBRACE, Literal = "}"},
                new Token() {Type = COMMA,  Literal = ","},
                new Token() {Type = SEMICOLON, Literal = ";"},
                new Token() {Type = EOF, Literal = ""},
            };

            TestCore(input, tests);
        }

        private void TestCore(string input, List<Token> tests)
        {
            var l = new Lexer(input);

            foreach (Token tt in tests) {
                Token tok = l.NextToken();
                output.WriteLine("===============================");
                output.WriteLine($"'{tt.Type}' - '{tok.Type}'");
                output.WriteLine($"'{tt.Literal}' - '{tok.Literal}'");
                Assert.Equal(tt.Type, tok.Type);
                Assert.Equal(tt.Literal, tok.Literal);
            }
        }
    }
}
