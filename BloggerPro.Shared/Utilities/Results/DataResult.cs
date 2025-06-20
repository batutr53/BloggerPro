using System.Text.Json.Serialization;

namespace BloggerPro.Shared.Utilities.Results;

public class DataResult<T> : Result, IDataResult<T>
{

    public DataResult(T data, bool success, string message, int httpStatusCode, string statusCode) : base(success, message == null ? null : new[] { message }, httpStatusCode, statusCode)
    {
        Data = data;
    }

    public DataResult(T data, bool success, string[] message, int httpStatusCode, string statusCode) : base(success, message, httpStatusCode, statusCode)
    {
        Data = data;
    }

    [JsonConstructor]
    public DataResult(T data, bool success, string[] message, int httpStatusCode, string statusCode, int count) : base(success, message, httpStatusCode, statusCode)
    {
        Data = data;
        Count = count;
    }

    public DataResult(T data, bool success) : base(success)
    {
        Data = data;
    }

    public DataResult(T data) : base(true)
    {
        Data = data;
    }

    public T Data { get; }
    public int Count { get; }
}
