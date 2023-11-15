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
    public List<AstNode> Args { get; set; } = new();
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
            var fnNameToken = consumer.consume();
            var fnName = fnNameToken!.Value.Value;
            fnDecl.Name = fnName;
            var next = consumer.consume(); // either left brace if no args, or left parenthesis
            
            if (next.Value!.Type == TokenType.LeftBrace) {
                // no-op. 
            } else if (next.Value!.Type == TokenType.LeftParenthesis) {
                // parse args
                var args = consumer.consumeUntil(TokenType.RightParenthesis);
                var argNodes = Parse(ref args);
                fnDecl.Args.AddRange(argNodes);
            } else {
                throw new UnexpectedTokenException(next.Value, [TokenType.LeftBrace, TokenType.LeftParenthesis], fnNameToken);
            }

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
                throw new UnexpectedTokenException(next.Value, [TokenType.LeftParenthesis], token);
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