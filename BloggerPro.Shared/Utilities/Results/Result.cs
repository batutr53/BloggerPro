using System.Text.Json.Serialization;

namespace BloggerPro.Shared.Utilities.Results;
public class Result : IResult
{
    [JsonConstructor]
    public Result(bool success, string[] message, int httpStatusCode, string statusCode) : this(success)
    {
        Message = message;
        HttpStatusCode = httpStatusCode;
        StatusCode = string.IsNullOrEmpty(statusCode) ? HttpStatusCode.ToString() : statusCode;
    }
    public Result(bool success)
    {
        Success = success;
        HttpStatusCode = success ? 200 : 400;
        StatusCode = HttpStatusCode.ToString();
        Message = success ? new[] { "success" } : new[] { "Operation Fail" };
    }
    public bool Success { get; }
    public string[] Message { get; }
    public int HttpStatusCode { get; }
    public string StatusCode { get; }
}