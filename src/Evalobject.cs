﻿using System.Collections.Generic;
using ast;
using menvironment;
using monkey;
using static evalobject.EvalObject;
using static evaluator.Evaluator;

namespace evalobject
{
    public interface EvalObject
    {
        static readonly string INTEGER_OBJ = "INTEGER";
        static readonly string BOOLEAN_OBJ = "BOOLEAN";
        static readonly string NULL_OBJ = "NULL";
        static readonly string RETURN_VALUE_OBJ = "RETURN_VALUE";
        static readonly string ERROR_OBJ = "ERROR_OBJ";
        static readonly string FUNCTION_OBJ = "FUNCTION";
        static readonly string STRING_OBJ = "STRING";
        static readonly string BUILTIN_OBJ = "BUILTIN";
        static readonly string DOT_OBJ = "DOT";


        string Type();
        string Inspect();
    }

    public class Integer : EvalObject
    {
        public int Value;
        public string Inspect() { return Value.ToString(); }
        public string Type() { return INTEGER_OBJ; }
    }

    public class Builtin : EvalObject
    {
        public delegate EvalObject BuiltinFunction(List<EvalObject> args);
        public BuiltinFunction Fn;
        public string Inspect() { return "builtin function"; }
        public string Type() { return BUILTIN_OBJ; }
    }

    public class StrObj : EvalObject
    {
        public string Value;
        public string Inspect() { return Value; }
        public string Type() { return STRING_OBJ; }
    }

    public class DotObj : EvalObject
    {
        public Node Value;
        public string Inspect() {
            DotBuilder builder = new DotBuilder();
            System.Console.WriteLine("geia");
            Value.Accept(builder);
            System.Console.WriteLine("geia");

            string d = "graph AST {\n";
            d += builder.build();
            d += "}";
            return d;
        }
        public string Type() { return DOT_OBJ; }
    }

    public class Function : EvalObject
    {
        public List<Identifier> Parameters;
        public BlockStatement Body;
        public MEnvironment Env;
        public string Type() { return FUNCTION_OBJ; }
        public string Inspect()
        {
            string input = "";
            List<string> args = new List<string>();

            foreach (var p in Parameters) {
                args.Add(p.String());
            }

            input += "fn";
            input += "(";
            input += System.String.Join(", ", args.ToArray());
            input += ") {\n";
            input += Body.String();
            input += "\n}";
            return input;
        }
    }


    public class ReturnValue : EvalObject
    {
        public EvalObject Value;
        public string Type() { return RETURN_VALUE_OBJ; }
        public string Inspect() { return Value.Inspect(); }
    }


    public class BoolObj : EvalObject
    {
        public bool Value;
        public string Inspect() { return Value.ToString(); }
        public string Type() { return BOOLEAN_OBJ; }
    }


    public class Null : EvalObject
    {
        public string Inspect() { return NULL_OBJ; }
        public string Type() { return "null"; }
    }


    public class Error : EvalObject
    {
        public string Message;
        public string Type() { return ERROR_OBJ; }
        public string Inspect() { return "ERROR: " + Message; }


        public Error(string msg)
        {
            Message = msg;
        }
    }
}
