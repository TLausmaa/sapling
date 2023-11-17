enum NodeType
{
    FnDecl,
    FnCall,
    Literal,
    Identifier,
    Operator
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

class IdentifierNode : AstNode
{
    public NodeType Type => NodeType.Identifier;
    public List<AstNode> Children { get; set; } = new();
    public string Name { get; set; } = "";
}

class OperatorNode : AstNode 
{
    public NodeType Type => NodeType.Operator;
    public List<AstNode> Children { get; set; } = new();
    public string Operator { get; set; } = "";
}

record struct TokenParseResult
{
    public AstNode Node { get; set; }
    public int Consumed { get; set; }
}

enum Context 
{
    FnDeclArgs,
    FnCallArgs
}

class AstParseContext
{
    public Token parent { get; init; }
    public Token previous { get; init; }
    public Context context { get; init; }
}

class Ast
{
    public List<AstNode> Build(List<Token> tokens)
    {
        return Parse(tokens, new TokenConsumer(tokens));
    }

    public List<AstNode> Parse(List<Token> tokens, TokenConsumer consumer, AstParseContext? context = null)
    {
        var nodes = new List<AstNode>();

        while (consumer.hasMore())
        {
            var result = ParseToken((Token)consumer.consume()!, consumer, context);
            nodes.Add(result.Node);
        }

        return nodes;
    }

    public TokenParseResult ParseToken(Token token, TokenConsumer consumer, AstParseContext? context = null)
    {
        Console.WriteLine($"AST: Parsing token '{token.Type}': '{token.Value}'");

        if (token.Type == TokenType.FnDecl)
        {
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
                var argNodes = Parse(args, new TokenConsumer(args), new AstParseContext() { parent = token, context = Context.FnDeclArgs });
                fnDecl.Args.AddRange(argNodes);
                consumer.consume(); // Left brace
            } else {
                throw new UnexpectedTokenException(next.Value, [TokenType.LeftBrace, TokenType.LeftParenthesis], fnNameToken);
            }

            var childTokens = consumer.consumeUntil(TokenType.RightBrace);
            var astNodes = Parse(childTokens, new TokenConsumer(childTokens));
            fnDecl.Children.AddRange(astNodes);
            return new TokenParseResult() { Node = fnDecl, Consumed = childTokens.Count + 3 }; // +3 for fnDecl, left brace, right brace
        }
        else if (token.Type == TokenType.Identifier)
        {
            Token? next = consumer.hasMore() ? consumer.peek() : null;

            if (next.HasValue && next.Value.Type == TokenType.LeftParenthesis)
            {
                var fnCall = new FnCallNode();
                fnCall.Name = token.Value;
                consumer.consume(); // Left parenthesis
                var childTokens = consumer.consumeUntil(TokenType.RightParenthesis);
                var astNodes = Parse(childTokens, new TokenConsumer(childTokens), new AstParseContext() { context = Context.FnCallArgs });
                fnCall.Children.AddRange(astNodes);
                return new TokenParseResult() { Node = fnCall, Consumed = childTokens.Count + 2 };
            }
            else if (token.Type == TokenType.Identifier && (context?.context == Context.FnDeclArgs || context?.context == Context.FnCallArgs))
            {
                return new TokenParseResult() 
                { 
                    Node = new IdentifierNode() { Name = token.Value },
                    Consumed = 0
                };
            }
            else
            {
                throw new UnexpectedTokenException(next, [TokenType.LeftParenthesis], token);
            }
        }
        else if (token.Type == TokenType.String)
        {
            return new TokenParseResult() 
            {
                Node = new LiteralNode 
                {
                    LiteralType = LiteralType.String,
                    Value = token.Value
                },
                Consumed = 0
            };
        }
        else if (token.Type == TokenType.Operator)
        {
            return new TokenParseResult() 
            {
                Node = new OperatorNode() { Operator = token.Value },
                Consumed = 0
            };
        }
        else if (token.Type == TokenType.Number)
        {
            return new TokenParseResult() 
            {
                Node = new LiteralNode
                {
                    LiteralType = LiteralType.Number,
                    Value = token.Value
                },
            };
        }
        throw new Exception($"Unhandled token type '{token.Type}': '{token.Value}' when parsing AST");
    }
}