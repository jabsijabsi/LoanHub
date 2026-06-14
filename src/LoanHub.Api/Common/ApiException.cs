namespace LoanHub.Api.Common;

/// <summary>Base class for expected, mappable application errors.</summary>
public class ApiException : Exception
{
    public ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}

public sealed class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound) { }
}

public sealed class ConflictException : ApiException
{
    public ConflictException(string message) : base(message, StatusCodes.Status409Conflict) { }
}

public sealed class AuthException : ApiException
{
    public AuthException(string message) : base(message, StatusCodes.Status401Unauthorized) { }
}
