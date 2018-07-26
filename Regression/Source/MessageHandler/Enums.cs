namespace MessageHandler
{
    public enum LogLevel
    {
        Fatal = 0,
        Error,
        Warning,
        Info,
        Debug,
        Exception
    }

    public enum MaxDifferences
    {
        Zero = 0,
        One,
        Two,
        Three,
        Unlimited
    }

    public enum ShouldlyEvaluation
    {
        Empty = 0,
        Null,
        NotNull
    }
}
