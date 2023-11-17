using System.Text;

public class UnexpectedTokenException : Exception
{
    static string form(Token? t, List<TokenType> expectedTypes, Token? previous = null)
    {
        return $"""
        Unexpected token {Format.Token(t)} at {Format.Location(t)}{(previous != null ? $" after {Format.Token(previous)}" : "")}.
        Expected {(expectedTypes.Count == 1 ? expectedTypes[0] : "one of [" + string.Join(", ", expectedTypes) + "]")}.
        """;
    }

    public UnexpectedTokenException(
        Token? token, 
        List<TokenType> expectedTypes, 
        Token? previous = null) 
        : base(form(token, expectedTypes, previous))
    {
    }
}