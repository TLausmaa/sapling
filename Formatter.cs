class Format
{
    public static string Token(Token? t)
    {
        return t == null
            ? "null"
            : $"{t.Value.Type}:'{t.Value.Value}'";
    }

    public static string Location(Token? t)
    {
        return t == null
            ? "null"
            : $"{t.Value.sourceLocation.Line}:{t.Value.sourceLocation.Column}";
    }
}