public enum TokenType
{
    Number,
    Operator,
    LeftParenthesis,
    RightParenthesis,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    String,
    FnDecl,
    VariableDecl,
    Identifier,
}

public readonly record struct Token
{
    public string Value { get; init; }
    public TokenType Type { get; init; }
    public SourceLocation sourceLocation { get; init; }
}

enum SkipReason
{
    NoSkip,
    Comment,
    String
}

class BufferConsumeState
{
    private bool isCommentOpen = false;
    private bool isStringOpen = false;

    public SkipReason DoSkip(char c)
    {
        if (c == '"' && !isCommentOpen)
        {
            isStringOpen = !isStringOpen;
            return SkipReason.String;
        }

        if (!isCommentOpen && !isStringOpen && c == '#')
        {
            isCommentOpen = true;
        }
        if (isCommentOpen && c == '\n')
        {
            isCommentOpen = false;
        }

        if (isCommentOpen) return SkipReason.Comment;
        if (isStringOpen) return SkipReason.String;
        return SkipReason.NoSkip;
    }
}

public record struct SourceLocation
{
    public int Line { get; set; }
    public int Column { get; set; }
}

class Tokenizer
{
    TokenType ResolveToken(string value)
    {
        if (value[0] == '"' && value[^1] == '"')
        {
            return TokenType.String;
        }
        else if (Char.IsLetter(value[0]))
        {
            return TokenType.Identifier;
        }
        else if (Char.IsDigit(value[0]))
        {
            return TokenType.Number;
        }
        else
        {
            return TokenType.Operator;
        }
    }

    void AddToken(ref List<Token> tokens, TokenType type, ref string current, SourceLocation location)
    {
        // Narrow down the type if possible
        if (type == TokenType.Identifier)
        {
            if (current == "fn") type = TokenType.FnDecl;
            if (current == "let") type = TokenType.VariableDecl;
        }

        if (!string.IsNullOrEmpty(current))
        {
            var loc = new SourceLocation() { Line = location.Line, Column = location.Column - current.Length };
            tokens.Add(new Token() { Type = type, Value = current, sourceLocation = loc });
            current = "";
        }
    }

    void AddToken(ref List<Token> tokens, TokenType type, string value, SourceLocation location)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var loc = new SourceLocation() { Line = location.Line, Column = location.Column - value.Length };
            tokens.Add(new Token() { Type = type, Value = value, sourceLocation = loc });
        }
    }

    void AddAndResolveToken(ref List<Token> tokens, ref string current, SourceLocation location)
    {
        if (string.IsNullOrEmpty(current)) return;
        var type = ResolveToken(current);
        AddToken(ref tokens, type, ref current, location);
    }

    void IncrementCurrent(ref string current, char c)
    {
        if (c != ' ' && c != '\n' && c != '\r' && c != '\t')
        {
            current += c;
        }
    }

    public List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        var current = "";
        var consumer = new BufferConsumeState();
        var location = new SourceLocation() { Line = 1, Column = 0 };
        var deferredLineIncrement = false;

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            location.Column++;

            if (deferredLineIncrement)
            {
                location.Line++;
                location.Column = 0;
                deferredLineIncrement = false;
            }

            if (c == '\n')
            {
                deferredLineIncrement = true;
            }

            if (c == '"' && consumer.DoSkip(c) == SkipReason.String && !string.IsNullOrEmpty(current))
            {
                current += c;
                AddToken(ref tokens, TokenType.String, ref current, location);
                continue;
            }

            if (consumer.DoSkip(c) == SkipReason.Comment)
            {
                continue;
            }

            if (consumer.DoSkip(c) == SkipReason.String)
            {
                current += c;
                continue;
            }

            switch (c)
            {
                case ',':
                    AddAndResolveToken(ref tokens, ref current, location);
                    break;
                case '(':
                    AddToken(ref tokens, TokenType.Identifier, ref current, location);
                    AddToken(ref tokens, TokenType.LeftParenthesis, c.ToString(), location);
                    break;
                case ')':
                    AddAndResolveToken(ref tokens, ref current, location);
                    AddToken(ref tokens, TokenType.RightParenthesis, c.ToString(), location);
                    break;
                case '{':
                    AddToken(ref tokens, TokenType.Identifier, ref current, location);
                    AddToken(ref tokens, TokenType.LeftBrace, c.ToString(), location);
                    break;
                case '}':
                    AddToken(ref tokens, TokenType.Identifier, ref current, location);
                    AddToken(ref tokens, TokenType.RightBrace, c.ToString(), location);
                    break;
                case '[':
                    AddToken(ref tokens, TokenType.Identifier, ref current, location);
                    AddToken(ref tokens, TokenType.LeftBracket, c.ToString(), location);
                    break;
                case ']':
                    AddAndResolveToken(ref tokens, ref current, location);
                    AddToken(ref tokens, TokenType.RightBracket, c.ToString(), location);
                    break;
                case '+':
                case '-':
                case '*':
                case '/':
                case '=':
                    AddToken(ref tokens, TokenType.Identifier, ref current, location);
                    AddToken(ref tokens, TokenType.Operator, c.ToString(), location);
                    break;
                case ' ':
                case '\n':
                case '\r':
                    AddToken(ref tokens, TokenType.Identifier, ref current, location);
                    break;
                default:
                    IncrementCurrent(ref current, c);
                    break;
            }
        }

        // Add last token if any
        if (current.Length > 0)
        {
            AddAndResolveToken(ref tokens, ref current, location);
        }

        return tokens;
    }
}

class TokenPrinter
{
    public static void Print(List<Token> tokens)
    {
        Console.WriteLine($"# Tokens are ({tokens.Count}):");
        Console.WriteLine("".PadRight(30, '='));
        foreach (var token in tokens)
        {
            var type = $"{token.Type}".PadRight(20, ' ');
            Console.WriteLine($"{(type + token.Value).PadRight(34, ' ')} @ {token.sourceLocation.Line}:{token.sourceLocation.Column}");
        }
        Console.WriteLine("");
    }
}
