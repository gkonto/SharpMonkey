using System;
using System.Collections.Generic;

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
        LET,
        MINUS,
        BANG,
        ASTERISK,
        SLASH,
        LT,
        GT,
        TRUE,
        FALSE,
        IF,
        ELSE,
        RETURN,
        EQ,
        NOT_EQ
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Literal { get; set; }

        public static TokenType LookupIdent(string ident)
        {
            if (keywords.TryGetValue(ident, out TokenType type)) {
                return type;
            }
            return TokenType.IDENT;
        }
     
        public static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            {"fn", TokenType.FUNCTION},
            {"let", TokenType.LET},
            {"true", TokenType.TRUE},
            {"false", TokenType.FALSE},
            {"if", TokenType.IF},
            {"else", TokenType.ELSE},
            {"return", TokenType.RETURN}
        };
    }
}
