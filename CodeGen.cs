using System.Text;

class CodeGenerator
{
    Dictionary<string, string> functionMap = new Dictionary<string, string>()
    {
        { "print", "console.log" },
        { "readfile", "readFileSync" },
    };

    public void DebugPrint(StringBuilder sb)
    {
        Console.WriteLine($"# Generated code:");
        Console.WriteLine("".PadRight(30, '='));
        Console.WriteLine(sb.ToString());
    }

    public StringBuilder Generate(Ast ast)
    {
        var sb = new StringBuilder();

        foreach (var node in ast.RootNodes)
        {
            sb.Append(Generate(node));
        }

        return sb;
    }

    public string Generate(AstNode node)
    {
        var sb = new StringBuilder();

        if (node is FnDeclNode)
        {
            var fnDecl = (FnDeclNode)node;
            sb.AppendLine($"function {fnDecl.Name}() {{");
            foreach (var child in node.Children)
            {
                sb.AppendLine(Generate(child));
            }
            sb.AppendLine($"}}");
        }
        else if (node is FnCallNode)
        {
            var fnCall = (FnCallNode)node;
            var args = new StringBuilder();
            foreach (var child in node.Children)
            {
                args.Append(Generate(child));
            }
            var fnName = fnCall.Name;
            if (functionMap.ContainsKey(fnName))
            {
                fnName = functionMap[fnName];
            }
            sb.Append($"{fnName}({args.ToString()})");
        }
        else if (node is LiteralNode)
        {
            var literal = (LiteralNode)node;
            sb.Append(literal.Value);
        }
        else
        {
            throw new Exception($"Unhandled node type '{node.Type}'");
        }

        return sb.ToString();
    }
}