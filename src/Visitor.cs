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

    public class ConvertIdentifiersVisitor : Visitor
    {
        List<FunctionParamIdentifier> identifiers;
        public ConvertIdentifiersVisitor(List<FunctionParamIdentifier> idents)
        {
            identifiers = idents;
        }

        private (int, bool) findIdentifierIndex(Identifier ident)
        {
            for (int i = 0; i < identifiers.Count; ++i) {
                if (identifiers[i].value == ident.value) {
                    return (i, true);
                }
            }
            return (-1, false);
        }

        public void visit(IntegerLiteral e) { /* Do nothing */}
        
        public void visit(InfixExpression e)
        {
            if (e.left is Identifier ident) {
                var (index, valid) = findIdentifierIndex(ident);
                if (valid) {
                    e.left = new FunctionParamIdentifier(ident, index);
                }
            } else {
                e.left.Accept(this);
            }

            if (e.right is Identifier ident2) {
                var (index, valid) = findIdentifierIndex(ident2);
                if (valid) {
                    e.right = new FunctionParamIdentifier(ident2, index);
                }
            } else {
                e.right.Accept(this);
            }
        }

        public void visit(Program e) {}
        public void visit(CallExpression e)
        {
            for (int i = 0; i < e.arguments.Count; ++i) {
                if (e.arguments[i] is Identifier ident) {
                    var (index, valid) = findIdentifierIndex(ident);
                    if (valid) {
                        e.arguments[i] = new FunctionParamIdentifier(ident, index);
                    }
                } else {
                    e.arguments[i].Accept(this);
                }
            }
        }

        public void visit(StringLiteral e) {}
        public void visit(FunctionLiteral e)
        {
            e.body.Accept(this);
        }

        public void visit(AstBool e){}
        public void visit(DotStatement e) {}
        public void visit(PrefixExpression e)
        {
            if (e.right is Identifier ident) {
                var (index, valid) = findIdentifierIndex(ident);
                if (valid) {
                    e.right = new FunctionParamIdentifier(ident, index);
                }
            }
            e.right.Accept(this);
             
        }

        public void visit(BlockStatement e)
        {
            foreach (Statement stmt in e.statements) {
                stmt.Accept(this);
            }
        }

        public void visit(IfExpression e)
        {
            if (e.condition is Identifier ident) {
                var (index, valid) = findIdentifierIndex(ident);
                if (valid) {
                    e.condition = new FunctionParamIdentifier(ident, index);
                }
            } else {
                e.condition.Accept(this);
            }
            e.consequence.Accept(this);
            e.alternative.Accept(this);
        }

        public void visit(ExpressionStatement e)
        {
            if (e.expression is Identifier ident) {
                var (index, valid) = findIdentifierIndex(ident);
                if (valid) {
                    e.expression = new FunctionParamIdentifier(ident, index);
                }
            }    
            e.expression.Accept(this);
        }

        public void visit(LetStatement e)
        {
            if (e.value is Identifier ident) {
                var (index, valid) = findIdentifierIndex(ident);
                if (valid) {
                    e.value = new FunctionParamIdentifier(ident, index);
                    return;
                }
            }
            e.value.Accept(this);
        }

        public void visit(Identifier e) { /* Do nothing */}
        public void visit(ReturnStatement e)
        {
            if (e.returnValue is Identifier ident) {
                var (index, valid) = findIdentifierIndex(ident);
                if (valid) {
                    e.returnValue = new FunctionParamIdentifier(ident, index);
                    return;
                }
            }
            e.returnValue.Accept(this);
        }
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
