namespace MessageHandler.Pocos
{
    public class LoggingDetails
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public System.Type CallingClass { get; set; }
    }
}
