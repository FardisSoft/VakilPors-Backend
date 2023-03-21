using System.Net;

namespace VakilPors.Shared.Response;

public class AppResponse
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }

    public AppResponse(HttpStatusCode statusCode, string message)
    {
        IsSuccess = statusCode switch
        {
            HttpStatusCode.OK => true,
            _ => false
        };
        StatusCode = statusCode;
        Message = message;
    }
}
public class AppResponse<TData> : AppResponse
{
    public TData Data { get; set; }

    public AppResponse(TData data, HttpStatusCode statusCode, string message)
        : base(statusCode, message)
        => Data = data;

}