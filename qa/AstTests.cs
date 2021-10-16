using System;
using Xunit;
using monkey;
using static monkey.TokenType;


namespace ast_test
{
    public class AstTest
    {
        [Fact]
        public void TestString()
        {
            Program program = new Program();
            program.statements.Add(new LetStatement()
                            {
                                token = new Token{Type = LET, Literal = "let"}, 
                                name = new Identifier{token = new Token{Type = IDENT, Literal = "myVar"},
                                                    value = "myVar"},
                                value = new Identifier{token = new Token{Type = IDENT, Literal = "anotherVar"},
                                                            value = "anotherVar"}
                            });
            Assert.Equal("let myVar = anotherVar;", program.String());
        }
    }
}
