using System.Net;

namespace SmartConfig.Common.Exceptions;

public class SmartConfigException : Exception
{
    public int StatusCode { get; set; }

    public SmartConfigException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = (int)statusCode;
    }
}