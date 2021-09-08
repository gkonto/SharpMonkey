using System;

namespace token
{
    public enum TokenType
    {
        ILLEGAL, //
        EOF,     //
        IDENT,   // add, foobar, x, y
        INT,     // 1234354
        ASSIGN,  // =
        PLUS,    // +
        COMMA,    // ,
        SEMICOLON, // ;
        LPAREN,   // (
        RPAREN,   // )
        LBRACE,  // {
        RBRACE,  // }
        FUNCTION, // 
        LET
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Literal { get; set; }
    }
}
