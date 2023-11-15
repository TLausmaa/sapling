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