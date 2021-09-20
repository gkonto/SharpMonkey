using System.Collections.Generic;
using token;

namespace ast
{
    public class Node 
    {
        public virtual string TokenLiteral() { return ""; }
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
    }
}