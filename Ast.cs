enum NodeType
{
    FnDecl,
    FnCall,
    Literal
}

enum LiteralType 
{
    String,
    Number
}

interface AstNode 
{
    public NodeType Type { get; }
    public List<AstNode> Children { get; set; }
}

class FnDeclNode : AstNode
{
    public NodeType Type => NodeType.FnDecl;
    public List<AstNode> Children { get; set; } = new();
    public string Name { get; set; } = "";
}

class FnCallNode : AstNode
{
    public NodeType Type => NodeType.FnCall;
    public List<AstNode> Children { get; set; } = new();
    public string Name { get; set; } = "";
}

class LiteralNode : AstNode
{
    public NodeType Type => NodeType.Literal;
    public List<AstNode> Children { get; set; } = new();
    public LiteralType LiteralType { get; set; }
    public string Value { get; set; } = "";
}

class TokenConsumer
{
    public List<Token> Tokens { get; set; }
    public int Index { get; set; } = 0;

    public TokenConsumer(List<Token> tokens, int index = 0)
    {
        Tokens = tokens;
        Index = index;
    }

    public Token? consume(int count = 1)
    {
        if (Index >= Tokens.Count)
        {
            return null;
        }
        var token = Tokens[Index];
        Index += count;
        return token;
    }

    public bool hasMore()
    {
        return Index < Tokens.Count;
    }

    public Token peek()
    {
        return Tokens[Index];
    }

    public Token peekNext()
    {
        return Tokens[Index + 1];
    }

    public List<Token> consumeUntil(TokenType type)
    {
        var tokens = new List<Token>();
        var tokenMaybe = consume();
        while (tokenMaybe.HasValue)
        {
            var token = tokenMaybe.Value;
            if (token.Type == type)
            {
                break;
            }
            tokens.Add(token);
            tokenMaybe = consume();
        }
        return tokens;
    }

    public void SetIndex(int i)
    {
        Index = i;
    }
}

record struct TokenParseResult
{
    public AstNode Node { get; set; }
    public int Consumed { get; set; }
}

class Ast
{
    public List<AstNode> RootNodes { get; set; } = new();

    public List<AstNode> Parse(ref List<Token> tokens)
    {
        var nodes = new List<AstNode>();
        for (int i = 0; i < tokens.Count; i++)
        {
            var result = ParseToken(tokens, i);
            nodes.Add(result.Node);
            i += result.Consumed;
        }
        return nodes;
    }

    public TokenParseResult ParseToken(List<Token> tokens, int i)
    {
        Token token = tokens[i];

        if (token.Type == TokenType.FnDecl)
        {
            var consumer = new TokenConsumer(tokens, i + 1);
            var fnDecl = new FnDeclNode();
            var fnName = consumer.consume()!.Value.Value;
            fnDecl.Name = fnName;
            consumer.consume(); // Skip left brace
            var childTokens = consumer.consumeUntil(TokenType.RightBrace);
            var astNodes = Parse(ref childTokens);
            fnDecl.Children.AddRange(astNodes);
            return new TokenParseResult() { Node = fnDecl, Consumed = childTokens.Count + 3 }; // +3 for fnDecl, left brace, right brace
        }
        else if (token.Type == TokenType.Identifier)
        {
            var consumer = new TokenConsumer(tokens, i + 1);
            var next = consumer.consume();

            if (next == null)
            {
                throw new Exception($"Next token was null. Current token was '{token.Type}': '{token.Value}'");
            }

            if (next.Value.Type == TokenType.LeftParenthesis)
            {
                var fnCall = new FnCallNode();
                fnCall.Name = token.Value;
                var childTokens = consumer.consumeUntil(TokenType.RightParenthesis);
                var astNodes = Parse(ref childTokens);
                fnCall.Children.AddRange(astNodes);
                return new TokenParseResult() { Node = fnCall, Consumed = childTokens.Count + 2 };
            }
            else
            {
                throw new Exception($"Unexpected token type '{next.Value.Type}' after identifier '{token.Value}'");
            }
        }
        else if (token.Type == TokenType.String)
        {
            return new TokenParseResult() {
                Node = new LiteralNode {
                    LiteralType = LiteralType.String,
                    Value = token.Value
                },
                Consumed = 1
            };
        }
        throw new Exception($"Unhandled token type '{token.Type}': '{token.Value}' when parsing into AST");
    }

    public void Build(List<Token> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            var result = ParseToken(tokens, i);
            RootNodes.Add(result.Node);
            i += result.Consumed;
        }
    }
}

class AstPrinter
{
    public static void Print(Ast ast)
    {
        Console.WriteLine($"# AST nodes are ({ast.RootNodes.Count}):");
        Console.WriteLine("".PadRight(30, '='));
        foreach (var node in ast.RootNodes)
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
            default:
                throw new Exception($"Unknown node type {node.Type}");
        }
    }
}