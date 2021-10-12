using System;
using lexer;
using token;
using ast;
using static token.TokenType;
using System.Collections.Generic;

namespace parser
{
    public enum Precedence
    {
        iota,
        LOWEST,  
        EQUALS, // == 
        LESSGREATER,  // > or <
        SUM, // +
        PRODUCT,  // *
        PREFIX, // -X and !X
        CALL   // myfunction(X)
    }

    public delegate Expression prefixParseFn();
    public delegate Expression infixParseFn(Expression e);

    public class Parser
    {
        lexer.Lexer lexer;
        Token curToken = new Token() {Type = ILLEGAL, Literal = ""};
        Token peekToken = new Token() {Type = ILLEGAL, Literal = ""};
        public List<string> errors { set; get; }
        IDictionary<TokenType, prefixParseFn> prefixParseFns = new Dictionary<TokenType, prefixParseFn>();
        IDictionary<TokenType, infixParseFn> infixParseFns = new Dictionary<TokenType, infixParseFn>();
        IDictionary<TokenType, Precedence> precedences = 
            new Dictionary<TokenType, Precedence>()
            {
                { EQ, Precedence.EQUALS },
                { NOT_EQ, Precedence.EQUALS },
                { LT, Precedence.LESSGREATER },
                { GT, Precedence.LESSGREATER },
                { PLUS, Precedence.SUM },
                { MINUS, Precedence.SUM },
                { SLASH, Precedence.PRODUCT },
                { ASTERISK, Precedence.PRODUCT },
                { LPAREN, Precedence.CALL}
            };

        public Parser(Lexer l)
        {
            errors = new List<string>();
            lexer = l;
            registerPrefix(IDENT, parseIdentifier);
            registerPrefix(INT, parseIntegerLiteral);
            registerPrefix(BANG, parsePrefixExpression);
            registerPrefix(MINUS, parsePrefixExpression);
            registerPrefix(TRUE, parseBoolean);
            registerPrefix(FALSE, parseBoolean);
            registerPrefix(LPAREN, parseGroupedExpression);
            registerPrefix(IF, parseIfExpression);
            registerPrefix(FUNCTION, parseFunctionLiteral);
            registerPrefix(STRING, parseStringLiteral);
        
            registerInfix(PLUS, parseInfixExpression);
            registerInfix(MINUS, parseInfixExpression);
            registerInfix(SLASH, parseInfixExpression);
            registerInfix(ASTERISK, parseInfixExpression);
            registerInfix(EQ, parseInfixExpression);
            registerInfix(NOT_EQ, parseInfixExpression);
            registerInfix(LT, parseInfixExpression);
            registerInfix(GT, parseInfixExpression);
            registerInfix(LPAREN, parseCallExpression);

            nextToken();
            nextToken();
        }

        public BlockStatement parseBlockStatement()
        {
            BlockStatement block = new BlockStatement() {token = curToken};
            nextToken();
            while (!curTokenIs(RBRACE) && !curTokenIs(EOF)) {
                Statement stmt = parseStatement();
                if (stmt != null) {
                    block.statements.Add(stmt);
                }
                nextToken();
            }
            return block;
        }

        public Expression parseStringLiteral()
        {
            return new StringLiteral() {token = curToken, Value = curToken.Literal};
        }

        public Expression parseFunctionLiteral()
        {
            FunctionLiteral lit = new FunctionLiteral() {};
            if (!expectPeek(LPAREN)) {
                return null;
            }

            lit.parameters = parseFunctionParameters();
            if (!expectPeek(LBRACE)) {
                return null;
            }

            lit.body = parseBlockStatement();
            return lit;
        }


        public List<Identifier> parseFunctionParameters()
        {
            List<Identifier> identifiers = new List<Identifier>();

            if (peekTokenIs(RPAREN)) {
                nextToken();
                return identifiers;
            }

            nextToken();

            Identifier ident = new Identifier() {token = curToken, value = curToken.Literal};
            identifiers.Add(ident);

            while (peekTokenIs(COMMA)) {
                nextToken();
                nextToken();
                Identifier ident2 = new Identifier() {token = curToken, value = curToken.Literal};
                identifiers.Add(ident2);
            }

            if (!expectPeek(RPAREN)) {
                return null;
            }

            return identifiers;
        }

        public Expression parseIfExpression()
        {
            IfExpression expression = new IfExpression() {token = curToken};
            if (!expectPeek(LPAREN)) {
                return null;
            }

            nextToken();
            expression.condition = parseExpression(Precedence.LOWEST);

            if (!expectPeek(RPAREN)) {
                return null;
            }

            if (!expectPeek(LBRACE)) {
                return null;
            }
            
            expression.consequence = parseBlockStatement();

            if (peekTokenIs(ELSE)) {
                nextToken();

                if (!expectPeek(LBRACE)) {
                    return null;
                }
                expression.alternative = parseBlockStatement();
            }

            return expression;
        }

        public Expression parseGroupedExpression()
        {
            nextToken();
            Expression exp = parseExpression(Precedence.LOWEST);
            if (!expectPeek(RPAREN)) {
                return null;
            }

            return exp;
        }


        public Expression parseBoolean()
        {
            return new ast.Boolean() {token = curToken, value = curTokenIs(TRUE)};
        }


        public Expression parseCallExpression(Expression fun)
        {
            CallExpression exp = new CallExpression() {t = curToken, function = fun, arguments = parseCallArguments()};
            return exp;    
        }


        public List<Expression> parseCallArguments()
        {
            List<Expression> args = new List<Expression>();

            if (peekTokenIs(RPAREN)) {
                nextToken();
                return args;
            }

            nextToken();

            args.Add(parseExpression(Precedence.LOWEST));
            while (peekTokenIs(COMMA)) {
                nextToken();
                nextToken();
                args.Add(parseExpression(Precedence.LOWEST));
            }

            if (!expectPeek(RPAREN)) {
                return null;
            }
            
            return args;
        }


        public Expression parseInfixExpression(Expression lhs)
        {
            InfixExpression expression = new InfixExpression() {
                token = curToken,
                Operator = curToken.Literal,
                left = lhs
            };

            Precedence precedence = curPrecedence();
            nextToken();
            expression.right = parseExpression(precedence);

            return expression;
        }


        public Expression parsePrefixExpression()
        {
            PrefixExpression expression = new PrefixExpression() {token = curToken, Operator = curToken.Literal};
            nextToken();
            expression.right = parseExpression(Precedence.PREFIX);
            return expression;
        }


        private void registerPrefix(TokenType tt, prefixParseFn fn)
        {
            prefixParseFns.Add(tt, fn);
        }


        private void registerInfix(TokenType tt, infixParseFn fn)
        {
            infixParseFns.Add(tt, fn);
        }


        public void noPrefixParseFnError(TokenType t)
        {
            string msg = $"no prefix parse function for {t} found";
            errors.Add(msg);
        }


        public Precedence peekPrecedence()
        {
            Precedence p = Precedence.LOWEST;
            precedences.TryGetValue(peekToken.Type, out p);
            return p;
        }


        public Precedence curPrecedence()  
        {
            Precedence p = Precedence.LOWEST;
            precedences.TryGetValue(curToken.Type, out p);
            return p;
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


        public Program ParseProgram()
        {
            Program program = new Program();

            while (curToken.Type != EOF) {
                Statement stmt = parseStatement();
                if (stmt != null) {
                    program.statements.Add(stmt);
                }
                nextToken();
            }
            return program;
        }

        public Expression parseIdentifier()
        {
            return new Identifier() {token = curToken, value = curToken.Literal };
        }

        public ReturnStatement parseReturnStatement()
        {
            ReturnStatement stmt = new ReturnStatement(curToken);
            nextToken();

            stmt.returnValue = parseExpression(Precedence.LOWEST);
            
            if (peekTokenIs(SEMICOLON)) {
                nextToken();
            }
            
            return stmt;
        }

        public Statement parseStatement()
        {
            switch (curToken.Type) {
                case LET:
                    return parseLetStatement();
                case RETURN:
                    return parseReturnStatement();
                default:
                    return parseExpressionStatement();
            }
        }

        public Expression parseIntegerLiteral()
        {
            IntegerLiteral lit = new IntegerLiteral() {token = curToken};
        
            try {
                int value = int.Parse(curToken.Literal);
                lit.value = value;
            } catch {
                errors.Add($"could not parse {curToken.Literal} as integer");
                return null;
            }

            return lit;
        }

        public Expression parseExpression(Precedence precedence)
        {
            prefixParseFn prefix;
            if (!prefixParseFns.TryGetValue(curToken.Type, out prefix)){ 
                noPrefixParseFnError(curToken.Type);
                return null;
            }

            var leftExp = prefix();

            while (!peekTokenIs(SEMICOLON) && precedence < peekPrecedence()) {
                infixParseFn infix;   
                if (!infixParseFns.TryGetValue(peekToken.Type, out infix)) {
                    return leftExp;
                }

                nextToken();
                leftExp = infix(leftExp);
            }

            return leftExp;
        }

        public ExpressionStatement parseExpressionStatement()
        {
            ExpressionStatement stmt = new ExpressionStatement() {token = curToken};    
            stmt.expression = parseExpression(Precedence.LOWEST);
            
            if (peekTokenIs(SEMICOLON)) {
                nextToken();
            }
                
            return stmt;
        }

        public LetStatement parseLetStatement()
        {
            LetStatement stmt = new LetStatement() {token = curToken};

            if (!expectPeek(IDENT)) {
                return null;
            }

            stmt.name = new Identifier() {token = curToken, value = curToken.Literal};

            if (!expectPeek(ASSIGN)) {
                return null;
            }

            nextToken();
            stmt.value = parseExpression(Precedence.LOWEST);

            if (peekTokenIs(SEMICOLON)) {
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
