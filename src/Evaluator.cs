using System;
using ast;
using System.Collections.Generic;
using menvironment;
using evalobject;

namespace evaluator
{
    public class Evaluator
    {
        public static Dictionary<string, Builtin> Builtins = new Dictionary<string, Builtin>() {
            {"len", new Builtin() {Fn = len} }
        };

        public static EvalObject len(List<EvalObject> args) 
        {
            if (args.Count != 1) {
                return new Error($"wrong number of arguments. got={args.Count}, want=1");
            }
            
            if (args[0] is evalobject.String str_arg) {
                return new Integer() { Value = str_arg.Value.Length };
            } else {
                return new Error($"argument to 'len' not supported, got {args[0].Type()}");
            }
        }

        public static readonly evalobject.Boolean TRUE = new evalobject.Boolean { Value = true };
        public readonly static evalobject.Boolean FALSE = new evalobject.Boolean { Value = false };
        public readonly static evalobject.Null  NULL = new evalobject.Null();

        public static evalobject.EvalObject Eval(Node node, MEnvironment env)
        {
            if (node is IntegerLiteral i) {
                return new Integer { Value = i.value };
            } else if (node is Program p) {
                return evalProgram(p, env);
            } else if (node is ExpressionStatement stmt) {
                return Eval(stmt.expression, env);
            } else if (node is ast.Boolean b) {
                return nativeBoolToBooleanObject(b.value);
            } else if (node is PrefixExpression prefexp) {
                EvalObject right = Eval(prefexp.right, env);
                return evalPrefixExpression(prefexp.Operator, right);
            } else if (node is ast.InfixExpression ie) {
                EvalObject left = Eval(ie.left, env);
                EvalObject right = Eval(ie.right, env);
                return evalInfixExpression(ie.Operator, left, right);
            } else if (node is ast.BlockStatement bs) {
                return evalBlockStatement(bs, env);
            } else if (node is ast.IfExpression ifexp) {
                return evalIfExpression(ifexp, env);
            } else if (node is ast.ReturnStatement rs) {
                EvalObject val = Eval(rs.returnValue, env);
                return new evalobject.ReturnValue{Value = val};
            } else if (node is ast.LetStatement ls) {
                EvalObject val = Eval(ls.value, env);
                if (isError(val)) {
                    return val;
                }
                env.Set(ls.name.value, val);
            } else if (node is ast.Identifier ident) {
                return evalIdentifier(ident, env);  
            } else if (node is ast.FunctionLiteral funlit) {
                List<Identifier> par = funlit.parameters;
                BlockStatement block = funlit.body;
                return new Function() { Parameters = par, Env = env, Body = block};
            } else if (node is ast.CallExpression ce) {
                EvalObject function = Eval(ce.function, env);
                if (isError(function)) {
                    return function;
                }

                List<EvalObject> args = evalExpressions(ce.arguments, env);
                if (args.Count == 1 && isError(args[0])) {
                    return args[0];
                }
                return applyFunction(function, args);
            } else if (node is ast.StringLiteral str) {
                return new evalobject.String() { Value = str.Value };
            }

            return null;
        }

        private static EvalObject applyFunction(EvalObject fn, List<EvalObject> args)
        {
            if (fn is Function fun) {
                MEnvironment extendedEnv = extendFunctionEnv(fun, args);
                EvalObject evaluated = Eval(fun.Body, extendedEnv);
                return unwrapReturnValue(evaluated);
            } else if (fn is Builtin builtin) {
                return builtin.Fn(args);
            } else {
                return new Error($"not a function: {fn.Type()}");
            }
        }

        private static MEnvironment extendFunctionEnv(Function fn, List<EvalObject> args)
        {
            MEnvironment env = new MEnvironment(fn.Env);
            for (int i = 0; i < fn.Parameters.Count; i++) {
                env.Set(fn.Parameters[i].value, args[i]);
            }
            return env;
        }

        private static EvalObject unwrapReturnValue(EvalObject obj)
        {
            if (obj is ReturnValue rv) {
                return rv.Value;
            }
            return obj;
        }



        private static List<EvalObject> evalExpressions(List<Expression> exps, MEnvironment env)
        {
            List<EvalObject> result = new List<EvalObject>();
            foreach (Expression e in exps) {
                EvalObject evaluated = Eval(e, env);
                if (isError(evaluated)) {
                    result.Add(evaluated);
                    return result;
                }
                result.Add(evaluated);
            }
            return result;
        }

        private static EvalObject evalIdentifier(ast.Identifier ident, MEnvironment env)
        {
            EvalObject o = env.Get(ident.value);
            if (o != null) {
                return o;
            }

            Builtin b;
            Builtins.TryGetValue(ident.value, out b);
            if (b != null) {
                return b;
            }

            return new Error($"identifier not found: {ident.value}");
        }


        private static bool isError(EvalObject o)
        {
            if (o != null) {
                return o.Type() == EvalObject.ERROR_OBJ;
            }
            return false;
        }


        private static EvalObject evalIfExpression(ast.IfExpression ie, MEnvironment env)
        {
            EvalObject condition = Eval(ie.condition, env);
            if (isTruthy(condition)) {
                return Eval(ie.consequence, env);
            } else if (ie.alternative != null) {
                return Eval(ie.alternative, env);
            } else {
                return NULL;
            }
        }


        private static EvalObject evalProgram(Program program, MEnvironment env)
        {
            EvalObject result = null;
            foreach (Statement s in program.statements) {
                result = Eval(s, env);
                if (result != null) {
                    if (result is ReturnValue rv) {
                        return rv.Value;
                    } else if (result is Error err) {
                        return err;
                    }
                }
            }
            return result;
        }


        private static EvalObject evalBlockStatement(BlockStatement bs, MEnvironment env)
        {
            EvalObject result = null;
            foreach (Statement s in bs.statements) {
                result = Eval(s, env);
                if (result != null) {
                    string rt = result.Type();
                    if (rt == EvalObject.RETURN_VALUE_OBJ || rt == EvalObject.ERROR_OBJ) {
                        return result;
                    }
                }
            }
            return result;
        }

        public static bool isTruthy(EvalObject obj)
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
        
        public static EvalObject evalIntegerInfixExpression(string op, EvalObject left, EvalObject right)
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
                return new evalobject.Error($"unknown operator: {left.Type()} {op} {right.Type()}");
            }
        }

        public static EvalObject evalInfixExpression(string op, EvalObject left, EvalObject right)
        {
            if (left.Type() == EvalObject.INTEGER_OBJ && right.Type() == EvalObject.INTEGER_OBJ) {
                return evalIntegerInfixExpression(op, left, right);
            } else if (op == "==") {
                return nativeBoolToBooleanObject(left == right);
            } else if (op == "!=") {
                return nativeBoolToBooleanObject(left != right);
            } else if (left.Type() != right.Type()) {
                return new evalobject.Error($"type mismatch: {left.Type()} {op} {right.Type()}");
            } else if (left.Type() == EvalObject.STRING_OBJ && right.Type() == EvalObject.STRING_OBJ) {
                return evalStringInfixExpression(op, left, right);
            } else {
                return new evalobject.Error($"unknown operator: {left.Type()} {op} {right.Type()}");
            }
        }


        public static EvalObject evalStringInfixExpression(string op, EvalObject left, EvalObject right)
        {
            if (op != "+") {
                return new Error($"unknown operator: {left.Type()} {op} {right.Type()}");
            }
            string leftVal = ((evalobject.String)left).Value;
            string rightVal = ((evalobject.String)right).Value;
            return new evalobject.String() {Value = leftVal + rightVal};
        }


        public static EvalObject evalPrefixExpression(string op, EvalObject right)
        {
            switch (op) {
                case "!":
                    return evalBangOperatorExpression(right);
                case "-":
                    return evalMinusPrefixOperatorExpression(right);
                default:
                    return new evalobject.Error($"unknown operator: {op}{right.Type()}");
            }
        }


        public static EvalObject evalBangOperatorExpression(EvalObject right)
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


        public static EvalObject evalMinusPrefixOperatorExpression(EvalObject right)
        {
            if (right.Type() != EvalObject.INTEGER_OBJ) {
                return new Error($"unknown operator: -{right.Type()}");
            }
            int value = ((Integer)right).Value;
            return new Integer{ Value = -value };
        }


        public static evalobject.Boolean nativeBoolToBooleanObject(bool input)
        {
            return input ? Evaluator.TRUE : Evaluator.FALSE;
        }


        public static EvalObject evalStatements(List<Statement> stmts, MEnvironment env)
        {
            EvalObject result = null;

            foreach(Statement stmt in stmts) {
                result = Eval(stmt, env);
                if (result is ReturnValue r) {
                    return r.Value;
                }
            }
            
            return result;
        }
    }
}
