using System.Runtime.Serialization;

namespace Shared.Validation;

[Serializable]
public class ExceptionError : Exception
{
    public string Code { get; }

    public ExceptionError(Exception innerException, string code, string message) : base(message, innerException)
    {
        Code = code;
    }

    public ExceptionError(string code, string message) : this(null, code, message)
    {
    }

    public ExceptionError(string message) : this(null, string.Empty, message)
    {
    }

    protected ExceptionError(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}