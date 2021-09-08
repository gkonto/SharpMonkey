using System;
using token;
using static token.TokenType;
using static System.Console;

namespace lexer
{
    public class Lexer
    {
        public string Input { get; set; }
        public int Position { get; set; }
        public int ReadPosition { get; set; }
        public byte Ch { get; set; }


        public Lexer(string input)
        {
            Input = input;
            Position = 0;
            ReadPosition = 0;
            Ch = 0;
            readChar();
        }

        public void readChar()
        {
            if (ReadPosition >= Input.Length) {
                Ch = 0;
            } else {
                Ch = (byte)Input[ReadPosition];
            }
            Position = ReadPosition;
            ReadPosition +=1 ;
        }

        public Token NextToken() 
        {
            Token tok = new Token();
            switch ((char)Ch)
            {
                case '=':
                    tok.Type = ASSIGN;
                    tok.Literal = "=";
                    break;
                case ';':
                    tok.Type = SEMICOLON;
                    tok.Literal = ";";
                    break;
                case '(':
                    tok.Type = LPAREN;
                    tok.Literal = "(";
                    break;
                case ')':
                    tok.Type = RPAREN;
                    tok.Literal = ")";
                    break;
                case ',':  
                    tok.Type = COMMA;
                    tok.Literal = ",";
                    break;
                case '+':
                    tok.Type = PLUS;
                    tok.Literal = "+";
                    break;
                case '{':
                    tok.Type = LBRACE;
                    tok.Literal = "{";
                    break;
                case '}':
                    tok.Type = RBRACE;
                    tok.Literal = "}";
                    break;
                default:
                    tok.Literal = "";
                    tok.Type = EOF;
                    break;
            }
            readChar();
            return tok;
        }
    }
}
