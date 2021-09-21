using System;
using lexer;
using token;
using ast;
using static token.TokenType;
using System.Collections.Generic;

#nullable enable

namespace parser
{
    public class Parser
    {
        lexer.Lexer lexer;
        Token curToken = new Token() {Type = ILLEGAL, Literal = ""};
        Token peekToken = new Token() {Type = ILLEGAL, Literal = ""};
        public List<string> errors { set; get; }


        public Parser(Lexer l)
        {
            errors = new List<string>();
            lexer = l;
            nextToken();
            nextToken();
        }

        public void peekError(TokenType t)
        {
            errors.Add($"expected next token to be {t}, got {peekToken.Type} instead");
        }

        public void nextToken()
        {
            curToken = peekToken;
            peekToken = lexer.NextToken();
        }

        public Program? ParseProgram()
        {
            Program program = new Program();

            while (curToken.Type != EOF) {
                Statement? stmt = parseStatement();
                if (stmt != null) {
                    program.statements.Add(stmt);
                }
                nextToken();
            }
            return program;
        }

        public ReturnStatement parseReturnStatement()
        {
            ReturnStatement stmt = new ReturnStatement(curToken);
            nextToken();
            
            while (!curTokenIs(SEMICOLON)) {
                nextToken();
            }
            
            return stmt;
        }

        public Statement? parseStatement()
        {
            switch (curToken.Type) {
                case LET:
                    return parseLetStatement();
                case RETURN:
                    return parseReturnStatement();
                default:
                    return null;
            }
        }


        public LetStatement? parseLetStatement()
        {
            LetStatement stmt = new LetStatement() {token = curToken};

            if (!expectPeek(IDENT)) {
                return null;
            }

            stmt.name = new Identifier() {token = curToken, value = curToken.Literal};

            if (!expectPeek(ASSIGN)) {
                return null;
            }

            //TODO: We're skipping the expression until we
            // encounter a semicolon
            while (!curTokenIs(SEMICOLON)) {
                nextToken();
            }

            return stmt;
        }

        public bool curTokenIs(TokenType t)
        {
            return curToken.Type == t;
        }

        public bool peekTokenIs(TokenType t)
        {
            return peekToken.Type == t;
        }

        public bool expectPeek(TokenType t)
        {
            if (peekTokenIs(t)) {
                nextToken();
                return true;
            } else {
                peekError(t);
                return false;
            }
        }        
    }
}
