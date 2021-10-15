using System.Collections.Generic;
using token;
using monkey;

namespace ast
{
    public abstract class Node 
    {
        public virtual string TokenLiteral() { return ""; }
        public virtual string String() { return ""; }
        public virtual string toDot(int depth) { return ""; }
        public abstract void Accept(Visitor v);
    }

    public abstract class Statement : Node
    {

    }

    public abstract class Expression : Node
    {
    
    }

    public class Program : Node
    {
        public List<Statement> statements = new List<Statement>();

        public override void Accept(Visitor v) { v.visit(this); }

        public override string toDot(int rank)
        {
            rank++;
            string d = $"{rank.ToString()}[label = {String()}];\n";
            foreach (Statement stmt in statements) {
                d += stmt.toDot(rank); 
            }

            return d; 
        }

        public override string TokenLiteral()
        {
            if (statements.Count > 0) {
                return statements[0].TokenLiteral();
            } else {
                return "";
            }
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

    public class CallExpression : Expression
    {
        public Token t;
        public Expression function;
        public List<Expression> arguments;
        public override string TokenLiteral()
        {
            return t.Literal;
        }

        public override string String()
        {
            string buffer = "";
            List<string> args = new List<string>();
            foreach (Expression a in arguments) {
                args.Add(a.String());
            }
            buffer += function.String();
            buffer += "(";
            buffer += System.String.Join(", ", args.ToArray());
            buffer += ")";

            return buffer;
        }
        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class StringLiteral : Expression
    {
        public Token token;
        public string Value;
        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            return token.Literal;
        }

        public override void Accept(Visitor v) { v.visit(this); }
    }

    public class FunctionLiteral : Expression
    {
        public Token token;
        public List<Identifier> parameters;
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

        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class InfixExpression : Expression
    {
        public Token token;
        public Expression left;
        public string Operator;
        public Expression right;

        
        public override string toDot(int id)
        {
            int parent = id;
            id++;
            string d = $"\t{id.ToString()} [label = \"{GetType().Name} - {TokenLiteral()}\"];\n";
            d += $"\t{parent} -- {id}\n";
            d += left.toDot(id);
            d += right.toDot(id);
            
            return d; 
        }

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
        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class AstBool : Expression
    {
        public Token token;
        public bool value;
        public override string TokenLiteral()
        {
            return token.Literal;
        }
        
        public override string toDot(int rank)
        {
            rank++;
            string d = $"{rank.ToString()} [label = {GetType().Name} - {String()}];\n";
            return d; 
        }

        public override string String()
        {
            return token.Literal;
        }

        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class DotStatement : Statement
    {
        public Token token;
        public Node right;
        public DotStatement(Token t)
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

            if (right != null) {
                buffer += right.String();
            }
            buffer += ";";

            return buffer;
        }

        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class PrefixExpression : Expression
    {
        public Token token;
        public string Operator;
        public Expression right;

        
        public override string toDot(int rank)
        {
            rank++;
            string d = $"{rank.ToString()} [label = {GetType().Name} - {String()}];\n";
            d += right.toDot(rank);
            return d; 
        }

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

        public override void Accept(Visitor v) { v.visit(this); }
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

        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class IfExpression : Expression
    {
        public Token token;
        public Expression condition;
        public BlockStatement consequence;
        public BlockStatement alternative;

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
        public override void Accept(Visitor v) { v.visit(this); }
    }

    public class ExpressionStatement : Statement
    {
        public Token token;
        public Expression expression { get; set; }
        
        public override string toDot(int rank)
        {
            rank++;
            string d = $"{rank.ToString()} [label = {GetType().Name} - {String()}];\n";
            d += expression.toDot(rank);
            return d; 
        }

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

        public override void Accept(Visitor v) { v.visit(this); }
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

        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class Identifier : Expression
    {
        public Token token;
        public string value;

        
        public override string toDot(int rank)
        {
            rank++;
            string d = $"{rank.ToString()} [label = {GetType().Name} - {String()}];\n";
            return d; 
        }

        public void expressionNode() {}
        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            return value;
        }

        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class IntegerLiteral : Expression
    {
        public Token token;
        public int value;
        public override string toDot(int id)
        {
            int parent = id;
            id++;
            string d = $"\t{id.ToString()} [label = \"{GetType().Name} - {String()}\"];\n";
            d += $"\t{parent} -- {id}\n";
            return d; 
        }
        public override string TokenLiteral()
        {
            return token.Literal;
        }

        public override string String()
        {
            return token.Literal;
        }
        public override void Accept(Visitor v) { v.visit(this); }

    }

    public class ReturnStatement : Statement
    {
        public Token token;
        public Expression returnValue;

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
        public override void Accept(Visitor v) { v.visit(this); }

    }
}