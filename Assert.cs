class Assert
{
    class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(string message) : base(message) { }
    }


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
            TokenPrinter.Print(actual);
            throw new Exception($"Expected {expected.Count} tokens but got {actual.Count}");
        }

        for (var i = 0; i < expected.Count; i++)
        {
            var expectedToken = expected[i];
            var actualToken = actual[i];

            if (expectedToken.Type != actualToken.Type || expectedToken.Value != actualToken.Value)
            {
                throw new UnexpectedTokenException($"Expected {expectedToken.Type}: '{expectedToken.Value}' but got {actualToken.Type}: '{actualToken.Value}' at index {i}");
            }
        }
    }
}