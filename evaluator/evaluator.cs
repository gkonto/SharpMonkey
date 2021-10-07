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
                return evalProgram(p);
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
            } else if (t == typeof(ast.BlockStatement)) {
                ast.BlockStatement bs = (ast.BlockStatement)node;
                return evalBlockStatement(bs);
            } else if (t == typeof(ast.IfExpression)) {
                ast.IfExpression ie = (ast.IfExpression)node;
                return evalIfExpression(ie);
            } else if (t == typeof(ast.ReturnStatement)) {
                ast.ReturnStatement rs = (ast.ReturnStatement)node;
                obj.Object val = Eval(rs.returnValue);
                return new obj.ReturnValue{Value = val};
            }

            return null;
        }

        private static obj.Object evalIfExpression(ast.IfExpression ie)
        {
            obj.Object condition = Eval(ie.condition);
            if (isTruthy(condition)) {
                return Eval(ie.consequence);
            } else if (ie.alternative != null) {
                return Eval(ie.alternative);
            } else {
                return NULL;
            }
        }

        private static obj.Object evalProgram(Program program)
        {
            obj.Object result = null;
            foreach (Statement s in program.statements) {
                result = Eval(s);
                Type t = result.GetType();
                if (t == typeof(ReturnValue)) {
                    ReturnValue rv = (ReturnValue)result;
                    return rv.Value;
                }
            }
            return result;
        }

        private static obj.Object evalBlockStatement(BlockStatement bs)
        {
            obj.Object result = null;
            foreach (Statement s in bs.statements) {
                result = Eval(s);
                if (result != null && result.Type() == obj.Object.RETURN_VALUE_OBJ) {
                    return result;
                }
            }
            return result;
        }

        public static bool isTruthy(obj.Object obj)
        {
            if (obj == NULL) {
                return false;
            } else if (obj == TRUE) {
                return true;
            } else if (obj == FALSE) {
                return false;
            } else {
                return true;
            }
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
                Type t = result.GetType();
                if (t == typeof(obj.ReturnValue)) {
                    obj.ReturnValue r = (obj.ReturnValue)result;
                    return r.Value;
                }
            }
            
            return result;
        }
    }
}
