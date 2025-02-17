class AstPrinter
{
    public static void Print(List<AstNode> nodes)
    {
        Console.WriteLine($"# AST nodes are ({nodes.Count}):");
        Console.WriteLine("".PadRight(30, '='));
        foreach (var node in nodes)
        {
            PrintNode(node);
        }
        Console.WriteLine("");
    }

    static void Print(int indent, string s)
    {
        Console.WriteLine(new string('-', indent) + s);
    }

    static void PrintNode(AstNode node, int indent = 0)
    {
        switch (node.Type)
        {
            case NodeType.FnDecl:
                Print(indent, $"FnDecl: '{((FnDeclNode)node).Name}'");
                foreach (var c in node.Children)
                {
                    PrintNode(c, indent + 1);
                }
                break;
            case NodeType.FnCall:
                Print(indent, $"FnCall: '{((FnCallNode)node).Name}'");
                foreach (var c in node.Children)
                {
                    PrintNode(c, indent + 1);
                }
                break;
            case NodeType.Literal:
                var literal = (LiteralNode)node;
                Print(indent, $"Literal ({literal.LiteralType}): {literal.Value}");
                break;
            case NodeType.Operator:
                var op = (OperatorNode)node;
                Print(indent, $"Operator: {op.Operator}");
                foreach (var c in node.Children)
                {
                    PrintNode(c, indent + 1);
                }
                break;
            case NodeType.Identifier:
                var identifier = (IdentifierNode)node;
                Print(indent, $"Identifier: {identifier.Name}");
                foreach (var c in node.Children)
                {
                    PrintNode(c, indent + 1);
                }
                break;
            default:
                throw new Exception($"Unknown node type {node.Type}");
        }
    }
}