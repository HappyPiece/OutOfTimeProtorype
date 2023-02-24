using System.Runtime.Serialization;

namespace OutOfTimePrototype.Exceptions;

public class AccessNotAllowedException: Exception
{
    public AccessNotAllowedException() { }
    protected AccessNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public AccessNotAllowedException(string? message) : base(message) { }
    public AccessNotAllowedException(string? message, Exception? innerException) : base(message, innerException) { }
}