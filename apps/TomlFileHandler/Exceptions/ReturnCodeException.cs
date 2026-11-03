namespace TomlFileHandler.Exceptions;

public class ReturnCodeException : Exception
{
    public int ReturnCode { get; set; }

    public ReturnCodeException(int returnCode, string message)
        : base(message)
    {
        ReturnCode = returnCode;
    }

    public ReturnCodeException(int returnCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ReturnCode = returnCode;
    }
}
