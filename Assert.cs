class Assert
{
    public static void AreEqual(int expected, int actual)
    {
        if (expected != actual)
        {
            throw new Exception($"Expected {expected} but got {actual}");
        }
    }

    public static void AreListsEqual(List<Token> expected, List<Token> actual)
    {
        if (expected.Count != actual.Count)
        {
            throw new Exception($"Expected {expected.Count} tokens but got {actual.Count}");
        }

        for (var i = 0; i < expected.Count; i++)
        {
            var expectedToken = expected[i];
            var actualToken = actual[i];

            if (expectedToken.Type != actualToken.Type)
            {
                throw new Exception($"Expected token type {expectedToken.Type} but got {actualToken.Type}");
            }

            if (expectedToken.Value != actualToken.Value)
            {
                throw new Exception($"Expected token value {expectedToken.Value} but got {actualToken.Value}");
            }
        }
    }
}