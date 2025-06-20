using System.Text.Json.Serialization;

namespace BloggerPro.Shared.Utilities.Results
{
    public class SuccessDataResult<T> : DataResult<T>
    {
        public SuccessDataResult(T data, string message, int httpStatusCode = 200, string statusCode = "200") : base(data, true, message == null ? null : new[] { message }, httpStatusCode, statusCode)
        {
        }

        public SuccessDataResult(T data, string[] message, int httpStatusCode = 200, string statusCode = "200") : base(data, true, message, httpStatusCode, statusCode)
        {
        }

        public SuccessDataResult(T data) : base(data, true, (string)null, 200, "200")
        {
        }

        [JsonConstructor]
        public SuccessDataResult(T data, int count) : base(data, true, null, 200, "200", count)
        {
        }

        // Aşağıdakiler genelde kullanılmıyor.
        public SuccessDataResult(string message, int httpStatusCode = 200, string statusCode = "200") : base(default, true, message == null ? null : new[] { message }, httpStatusCode, statusCode)
        {

        }

        public SuccessDataResult(string[] message, int httpStatusCode = 200, string statusCode = "200") : base(default, true, message, httpStatusCode, statusCode)
        {

        }

        public SuccessDataResult() : base(default, true)
        {

        }
    }
}
