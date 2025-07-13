namespace SmartConfig.Api.Models;

public class ApiResponse<T>
{
    public T Response { get; set; }
}

public class ApiErrorResponse
{
    public List<ApiError> Errors { get; set; }
}

public class ApiError
{
    public string Message { get; set; }
    public string ErrorCode { get; set; }
}