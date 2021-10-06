using System;
using ast;
using obj;
using System.Collections.Generic;


namespace evaluator
{
    public class Evaluator
    {
        public static readonly obj.Boolean TRUE = new obj.Boolean { Value = true };
        public readonly static obj.Boolean FALSE = new obj.Boolean { Value = false };
        public readonly static obj.Null  NULL = new obj.Null();
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
            } else if (t == typeof(ast.Boolean)) {
                ast.Boolean b = (ast.Boolean)node;
                return nativeBoolToBooleanObject(b.value);
            } else if (t == typeof(ast.PrefixExpression)) {
                PrefixExpression p = (ast.PrefixExpression)node;
                obj.Object right = Eval(p.right);
                return evalPrefixExpression(p.Operator, right);
            } else if (t == typeof(ast.InfixExpression)) {
                ast.InfixExpression ie = (InfixExpression)node;
                obj.Object left = Eval(ie.left);
                obj.Object right = Eval(ie.right);
                return evalInfixExpression(ie.Operator, left, right);
            }

            return null;
        }
        
        public static obj.Object evalIntegerInfixExpression(string op, obj.Object left, obj.Object right)
        {
            int leftVal = ((Integer)left).Value;
            int rightVal = ((Integer)right).Value;

            if (op == "+") {
                return new Integer {Value = leftVal + rightVal};
            } else if (op == "-") {
                return new Integer {Value = leftVal - rightVal};
            } else if (op == "*") {
                return new Integer {Value = leftVal * rightVal};
            } else if (op == "/") {
                return new Integer {Value = leftVal / rightVal};
            } else if (op == "<") {
                return nativeBoolToBooleanObject(leftVal < rightVal);
            } else if (op == ">") {
                return nativeBoolToBooleanObject(leftVal > rightVal);
            } else if (op == "==") {
                return nativeBoolToBooleanObject(leftVal == rightVal);
            } else if (op == "!=") {
                return nativeBoolToBooleanObject(leftVal != rightVal);
            } else {
                return NULL;
            }
        }

        public static obj.Object evalInfixExpression(string op, obj.Object left, obj.Object right)
        {
            if (left.Type() == obj.Object.INTEGER_OBJ && right.Type() == obj.Object.INTEGER_OBJ) {
                return evalIntegerInfixExpression(op, left, right);
            } else if (op == "==") {
                return nativeBoolToBooleanObject(left == right);
            } else if (op == "!=") {
                return nativeBoolToBooleanObject(left != right);
            } else {
                return NULL;
            }
        }

        public static obj.Object evalPrefixExpression(string op, obj.Object right)
        {
            switch (op) {
                case "!":
                    return evalBangOperatorExpression(right);
                case "-":
                    return evalMinusPrefixOperatorExpression(right);
                default:
                    return NULL;
            }
        }

        public static obj.Object evalBangOperatorExpression(obj.Object right)
        {
            if (right ==  TRUE) {
                return FALSE;
            } else if (right == FALSE) {
                return TRUE;
            } else if (right == NULL) {
                return TRUE;
            } else {
                return FALSE;
            }
        }

        public static obj.Object evalMinusPrefixOperatorExpression(obj.Object right)
        {
            if (right.Type() != obj.Object.INTEGER_OBJ) {
                return NULL;
            }
            int value = ((Integer)right).Value;
            return new Integer{ Value = -value };
        }


        public static obj.Boolean nativeBoolToBooleanObject(bool input)
        {
            return input ? Evaluator.TRUE : Evaluator.FALSE;
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
