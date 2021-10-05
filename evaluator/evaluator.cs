using System;
using ast;
using obj;
using System.Collections.Generic;


namespace evaluator
{
    public class Evaluator
    {
        public static obj.Object Eval(Node node)
        {
            Type t = node.GetType();
            if (t == typeof(IntegerLiteral)) {
                return new Integer {Value = ((IntegerLiteral)node).value};
            } else if (t == typeof(Program)) {
                Program p = (Program)node;
                return evalStatements(p.statements);
            } else if (t == typeof(ExpressionStatement)) {
                ExpressionStatement stmt = (ExpressionStatement)node;
                return Eval(stmt.expression);
            }

            return null;
        }

        public static obj.Object evalStatements(List<Statement> stmts)
        {
            obj.Object result = null;

            foreach(Statement stmt in stmts) {
                result = Eval(stmt);
            }
            
            return result;
        }
    }
}
