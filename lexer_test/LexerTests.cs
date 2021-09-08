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

            var l = new Lexer(input);
            
            foreach (Token tt in tests) {
                Token tok = l.NextToken();
                Assert.Equal(tt.Type, tok.Type);
                Assert.Equal(tt.Literal, tok.Literal);
            }
        }
    }
}
