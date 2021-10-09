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
            ReadPosition += 1;
        }

        public string readIdentifier()
        {
            int start = Position;

            while (isLetter()) {
                readChar();
            }

            //return new string($"{start} {Input.Substring(start, Position)}");
            return Input.Substring(start, Position - start);
        }

        private bool isLetter()
        {
            return 'a' <= Ch && Ch <= 'z' || 'A' <= Ch && Ch <= 'Z' || Ch == '_';
        }

        public void skipWhitespace()
        {
            while (Ch == ' ' || Ch == '\t' || Ch == '\n' || Ch == '\r') {
                readChar();
            }
        }

        public string readNumber()
        {
            int start = Position;
            while (isDigit()) {
                readChar();
            }
            return Input.Substring(start, Position - start);
        }

        public bool isDigit()
        {
            return '0' <= Ch && Ch <= '9';
        }

        public byte peekChar()
        {
            if (ReadPosition >= Input.Length) {
                return 0;
            } else {
                return (byte)Input[ReadPosition];
            }
        }

        private string readString()
        {
            Position += 1;
            var start = Position;

            while (true) {
                readChar();
                if (Ch == '"' || Ch == 0) {
                    break;
                }
            }
            return Input.Substring(start, Position - start);
        }

        public Token NextToken() 
        {
            Token tok = new Token();

            skipWhitespace();

            switch ((char)Ch)
            {
                case '=':
                    if (peekChar() == '=') {
                        var ch = Ch;
                        readChar();
                        tok.Type = EQ;
                        tok.Literal = "==";
                    } else {
                        tok.Type = ASSIGN;
                        tok.Literal = "=";                      
                    }
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
                case '-':
                    tok.Type = MINUS;
                    tok.Literal = "-";
                    break;
                case '!':
                    if (peekChar() == '=') {
                        var ch = Ch;
                        readChar();
                        tok.Type = NOT_EQ;
                        tok.Literal = "!=";
                    } else {
                        tok.Type = BANG;
                        tok.Literal = "!";
                    }
                    break;
                case '/':
                    tok.Type = SLASH;
                    tok.Literal = "/";
                    break;
                case '*':
                    tok.Type = ASTERISK;
                    tok.Literal = "*";
                    break;
                case '<':
                    tok.Type = LT;
                    tok.Literal = "<";
                    break;
                case '>':
                    tok.Type = GT;
                    tok.Literal = ">";
                    break;
                case '\0':
                    tok.Literal = "";
                    tok.Type = EOF;
                    break;
                case '"':
                    tok.Type = STRING;
                    tok.Literal = readString();
                    break;
                default:
                    if (isLetter()) {
                        tok.Literal = readIdentifier();
                        tok.Type = Token.LookupIdent(tok.Literal);
                        return tok;      
                    } else if (isDigit()) {
                        tok.Type = INT;
                        tok.Literal = readNumber();
                        return tok;
                    } else {
                        tok.Type = ILLEGAL;
                        tok.Literal = System.Text.Encoding.ASCII.GetString(new[]{Ch});
                    }
                    break;
            }
            readChar();
            return tok;
        }
    }
}
