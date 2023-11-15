using System.Text;

public class UnexpectedTokenException : Exception
{
    static string form(Token t, List<TokenType> expectedTypes, Token? previous = null)
    {
        return $"""
        Unexpected token {t.Type}:'{t.Value}' at line {t.sourceLocation.Line}{(previous != null ? $" after {previous.Value.Type}:'{previous.Value.Value}'" : "")}.
        Expected {(expectedTypes.Count == 1 ? expectedTypes[0] : "one of [" + string.Join(", ", expectedTypes) + "]")}.
        """;
    }

    public UnexpectedTokenException(
        Token token, 
        List<TokenType> expectedTypes, 
        Token? previous = null) 
        : base(form(token, expectedTypes, previous))
    {
    }
}