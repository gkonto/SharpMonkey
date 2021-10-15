using ast;
using System.Collections.Generic;

namespace monkey
{
    public interface Visitor
    {
        void visit(IntegerLiteral e);
        void visit(InfixExpression e);
        void visit(Program e);
        void visit(CallExpression e);
        void visit(StringLiteral e);
        void visit(FunctionLiteral e);
        void visit(AstBool e);
        void visit(DotStatement e);
        void visit(PrefixExpression e);
        void visit(BlockStatement e);
        void visit(IfExpression e);
        void visit(ExpressionStatement e);
        void visit(LetStatement e);
        void visit(Identifier e);
        void visit(ReturnStatement e);

    }

    public class DotBuilder : Visitor
    {
        Dictionary<Node, int> ids = new Dictionary<Node, int>();
        List<(Node, Node)> edges = new List<(Node, Node)>();
        int current_id = 0;
        public string build()
        {
            string input = "";
            foreach (var entry in ids) {
                Node key = entry.Key;
                int id = entry.Value;
                input += $"\t{id} [label = \"{key.GetType().Name} - {key.TokenLiteral()}\"]\n";
            }
            foreach (var entry in edges) {
                Node parent = entry.Item1;
                Node child = entry.Item2;
                input += $"\t{ids[parent]} -- {ids[child]}\n";
            }
            return input;
        }
        public void visit(IntegerLiteral e) 
        {
            ids.Add(e, ++current_id);
        }

        public void visit(InfixExpression e)
        {
            e.left.Accept(this);
            e.right.Accept(this);
            ids.Add(e, ++current_id);
            edges.Add((e, e.left));
            edges.Add((e, e.right));
        }

        public void visit(Program e)
        {
            foreach (var p in e.statements) {
                e.Accept(this);
            }
            ids.Add(e, ++current_id);
            foreach (var p in e.statements) {
                edges.Add((e, p));
            }
        }

        public void visit(CallExpression e)
        {
            foreach( var a in e.arguments) {
                a.Accept(this);
            }
            e.function.Accept(this);
            ids.Add(e, ++current_id);
            foreach (var a in e.arguments) {
                edges.Add((e, a));
            }
            edges.Add((e, e.function));
        }

        public void visit(StringLiteral e)
        {
            ids.Add(e, ++current_id);
        }
        
        public void visit(FunctionLiteral e)
        {
            foreach (var s in e.parameters) {
                s.Accept(this);
            }
            e.body.Accept(this);
            ids.Add(e, ++current_id);
            foreach(var s in e.parameters) {
                edges.Add((e, s));
            }
            edges.Add((e, e.body));
        }

        public void visit(AstBool e)
        {
            ids.Add(e, ++current_id);
        }

        public void visit(DotStatement e)
        {
            ids.Add(e, ++current_id);
        }

        public void visit(PrefixExpression e)
        {
            e.right.Accept(this);
            ids.Add(e, ++current_id);
            edges.Add((e, e.right));
        }

        public void visit(BlockStatement e)
        {
            foreach (var s in e.statements) {
                s.Accept(this);
            }
            ids.Add(e, ++current_id);
            foreach (var s in e.statements) {
                edges.Add((e, s));
            }
        }

        public void visit(IfExpression e)
        {
            e.condition.Accept(this);
            e.consequence.Accept(this);
            e.alternative.Accept(this);
            ids.Add(e, ++current_id);
            edges.Add((e, e.condition));
            edges.Add((e, e.consequence));
            edges.Add((e, e.alternative));
        }

        public void visit(ExpressionStatement e)
        {
            e.expression.Accept(this);
            ids.Add(e, ++current_id);
            edges.Add((e, e.expression));
        }

        public void visit(LetStatement e)
        {
            e.name.Accept(this);
            e.value.Accept(this);
            ids.Add(e, ++current_id);
            edges.Add((e, e.name));
            edges.Add((e, e.value));
        }
        
        public void visit(Identifier e)
        {
            ids.Add(e, ++current_id);
        }

        public void visit(ReturnStatement e) 
        {
            e.returnValue.Accept(this);
            ids.Add(e, ++current_id);
            edges.Add((e, e.returnValue));
        }

    }
}
