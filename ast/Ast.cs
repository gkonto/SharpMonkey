using System.Collections.Generic;
using token;

#nullable enable

namespace ast
{
    public abstract class Node 
    {
        public virtual string TokenLiteral() { return ""; }
        public virtual string String() { return ""; }
    }

    public class Statement : Node
    {

    }

    public class Expression : Node
    {

    }

    public class Program 
    {
        public List<Statement> statements = new List<Statement>();

        public string TokenLiteral()
        {
            if (statements.Count > 0) {
                return statements[0].TokenLiteral();
            } else {
                return "";
            }
        }

        public string String()
        {
            string buffer = "";

            foreach (Statement s in statements) {
                buffer += s.String();
            }
            return buffer;
        }
    }

    public class FunctionLiteral : Expression
    {
        public Token token;
        public List<Identifier>? parameters;
        public BlockStatement body;

        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string input = "";

            input += TokenLiteral();
            input += "(";
            for (int i = 0; i < parameters.Count; ++i) {
                input += parameters[i].String();
                if (i != parameters.Count - 1) {
                    input += ", ";
                }
            }
            input += ")";
            input += body.String();

            return input;
        } 
    }

    public class InfixExpression : Expression
    {
        public Token token;
        public Expression left;
        public string Operator;
        public Expression right;

        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string input = "";

            input += "(";
            input += left.String();
            input += " "  + Operator + " ";
            input += right.String();
            input += ")";

            return input;
        }
    }

    public class Boolean : Expression
    {
        public Token token;
        public bool value;
        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            return token.Literal;
        }
    }

    public class PrefixExpression : Expression
    {
        public Token token;
        public string Operator;
        public Expression right;

        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string input = "";
            input += "(";
            input += Operator;
            input += right.String();
            input += ")";
            return input;
        }
    }

    public class BlockStatement : Statement
    {
        public Token token;
        public List<Statement> statements = new List<Statement>();

        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string buffer = "";
            foreach (Statement s in statements) {
                buffer += s.String();
            }
            return buffer;
        }
    }

    public class IfExpression : Expression
    {
        public Token token;
        public Expression condition;
        public BlockStatement consequence;
        public BlockStatement? alternative;

        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string buffer = "";
            buffer += "if";
            buffer += condition.String();
            buffer += " ";
            buffer += consequence.String();
            if (alternative != null) {
                buffer += "else";
                buffer += alternative.String();
            }
            return buffer;
        }
    }

    public class ExpressionStatement : Statement
    {
        public Token token;
        public Expression expression { get; set; }
        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string buffer = "";

            if (expression != null) {
                buffer += expression.String();
            }

            return buffer;
        }

    }

    public class LetStatement : Statement
    {
        public Token token;
        public Identifier name;
        public Expression value;

        public void statementNode() {}
        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string buffer = "";
            buffer += TokenLiteral() + " ";
            buffer += name.String();
            buffer += " = ";
            
            if (value != null) {
                buffer += value.String();
            }
            buffer += ";";

            return buffer;
        }
    }

    public class Identifier : Expression
    {
        public Token token;
        public string value;

        public void expressionNode() {}
        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string buffer = "";
            buffer += value;
            return buffer;
        }
    }

    public class IntegerLiteral : Expression
    {
        public Token token;
        public int value;

        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            return token.Literal;
        }
    }

    public class ReturnStatement : Statement
    {
        Token token;
        Expression returnValue;

        public ReturnStatement(Token t)
        {
            token = t;
        }

        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            string buffer = "";
            buffer += TokenLiteral() + " ";

            if (returnValue != null) {
                buffer += returnValue.String();
            }
            buffer += ";";

            return buffer;
        }

    }
}