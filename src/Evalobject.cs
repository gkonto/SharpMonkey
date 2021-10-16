using System.Collections.Generic;
using static monkey.TokenType;

namespace monkey
{
    public enum ObjectTypes
    {
        INTEGER,
        BOOLEAN,
        NULL,
        RETURN_VALUE,
        ERROR,
        FUNCTION,
        STRING,
        BUILTIN,
        DOT
    }
    public interface EvalObject
    {
        ObjectTypes Type();
        string Inspect();
    }

    public class Integer : EvalObject
    {
        public int Value;
        public string Inspect() { return Value.ToString(); }
        public ObjectTypes Type() { return ObjectTypes.INTEGER; }
    }

    public class Builtin : EvalObject
    {
        public delegate EvalObject BuiltinFunction(List<EvalObject> args);
        public BuiltinFunction Fn;
        public string Inspect() { return "builtin function"; }
        public ObjectTypes Type() { return ObjectTypes.BUILTIN; }
    }

    public class StrObj : EvalObject
    {
        public string Value;
        public string Inspect() { return Value; }
        public ObjectTypes Type() { return ObjectTypes.STRING; }
    }

    public class DotObj : EvalObject
    {
        public Node Value;
        public string Inspect() {
            DotBuilder builder = new DotBuilder();
            Value.Accept(builder);

            string d = "graph AST {\n";
            d += builder.build();
            d += "}";
            return d;
        }
        public ObjectTypes Type() { return ObjectTypes.DOT; }
    }

    public class Function : EvalObject
    {
        public List<Identifier> Parameters;
        public BlockStatement Body;
        public MEnvironment Env;
        public ObjectTypes Type() { return ObjectTypes.FUNCTION; }
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
        public ObjectTypes Type() { return ObjectTypes.RETURN_VALUE; }
        public string Inspect() { return Value.Inspect(); }
    }


    public class BoolObj : EvalObject
    {
        public bool Value;
        public string Inspect() { return Value.ToString(); }
        public ObjectTypes Type() { return ObjectTypes.BOOLEAN; }
    }


    public class Null : EvalObject
    {
        public string Inspect() { return "null"; }
        public ObjectTypes Type() { return ObjectTypes.NULL; }
    }


    public class Error : EvalObject
    {
        public string Message;
        public ObjectTypes Type() { return ObjectTypes.ERROR; }
        public string Inspect() { return "ERROR: " + Message; }


        public Error(string msg)
        {
            Message = msg;
        }
    }
}
