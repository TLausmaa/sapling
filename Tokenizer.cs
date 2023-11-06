enum TokenType
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
    Identifier
}

readonly record struct Token
{
    public string Value { get; init; }
    public TokenType Type { get; init; }
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

    void AddToken(ref List<Token> tokens, TokenType type, ref string current)
    {
        // Narrow down the type if possible
        if (type == TokenType.Identifier)
        {
            if (current == "fn") type = TokenType.FnDecl;
            if (current == "let") type = TokenType.VariableDecl;
        }

        if (!string.IsNullOrEmpty(current))
        {
            tokens.Add(new Token() { Type = type, Value = current });
            current = "";
        }
    }

    void AddToken(ref List<Token> tokens, TokenType type, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            tokens.Add(new Token() { Type = type, Value = value });
        }
    }

    void AddAndResolveToken(ref List<Token> tokens, ref string current)
    {
        var type = ResolveToken(current);
        AddToken(ref tokens, type, ref current);
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

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (c == '"' && consumer.DoSkip(c) == SkipReason.String && !string.IsNullOrEmpty(current))
            {
                current += c;
                AddToken(ref tokens, TokenType.String, ref current);
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
                    AddAndResolveToken(ref tokens, ref current);
                    break;
                case '(':
                    AddToken(ref tokens, TokenType.Identifier, ref current);
                    AddToken(ref tokens, TokenType.LeftParenthesis, c.ToString());
                    break;
                case ')':
                    AddToken(ref tokens, TokenType.Identifier, ref current);
                    AddToken(ref tokens, TokenType.RightParenthesis, c.ToString());
                    break;
                case '{':
                    AddToken(ref tokens, TokenType.Identifier, ref current);
                    AddToken(ref tokens, TokenType.LeftBrace, c.ToString());
                    break;
                case '}':
                    AddToken(ref tokens, TokenType.Identifier, ref current);
                    AddToken(ref tokens, TokenType.RightBrace, c.ToString());
                    break;
                case '[':
                    AddToken(ref tokens, TokenType.Identifier, ref current);
                    AddToken(ref tokens, TokenType.LeftBracket, c.ToString());
                    break;
                case ']':
                    AddAndResolveToken(ref tokens, ref current);
                    AddToken(ref tokens, TokenType.RightBracket, c.ToString());
                    break;
                case '+':
                case '-':
                case '*':
                case '/':
                case '=':
                    AddToken(ref tokens, TokenType.Identifier, ref current);
                    AddToken(ref tokens, TokenType.Operator, c.ToString());
                    break;
                case ' ':
                case '\n':
                case '\r':
                    AddToken(ref tokens, TokenType.Identifier, ref current);
                    break;
                default:
                    IncrementCurrent(ref current, c);
                    break;
            }
        }

        // Add last token if any
        if (current.Length > 0)
        {
            AddAndResolveToken(ref tokens, ref current);
        }

        return tokens;
    }
}
