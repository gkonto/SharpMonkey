using System;

namespace obj
{
    public interface Object
    {
        static readonly string INTEGER_OBJ = "INTEGER";
        static readonly string BOOLEAN_OBJ = "BOOLEAN";
        static readonly string NULL_OBJ = "NULL";

        string Type();
        string Inspect();
    }

    public class Integer : Object
    {
        public int Value;
        public string Inspect() { return Value.ToString(); }
        public string Type() { return Object.INTEGER_OBJ; }
    }

    public class Boolean : Object
    {
        public bool Value;
        public string Inspect() { return Value.ToString(); }
        public string Type() { return Object.BOOLEAN_OBJ; }
    }

    class Null : Object
    {
        public string Inspect() { return Object.NULL_OBJ; }
        public string Type() { return "null"; }
    }
}
