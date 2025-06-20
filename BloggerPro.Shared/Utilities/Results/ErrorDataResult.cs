namespace BloggerPro.Shared.Utilities.Results
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        public ErrorDataResult(T data, string message, int httpStatusCode = 400, string statusCode = "400") : base(data, false, message == null ? null : new[] { "400", message }, httpStatusCode, statusCode)
        {
        }

        public ErrorDataResult(T data, string[] message, int httpStatusCode = 400, string statusCode = "400") : base(data, false, message, httpStatusCode, statusCode)
        {
        }

        public ErrorDataResult(T data) : base(data, false, (string)null, 400, "400")
        {
        }

        // Aşağıdakiler genelde kullanılmıyor.
        public ErrorDataResult(string message, int httpStatusCode = 400, string statusCode = "400") : base(default, false, message == null ? null : new[] { "400", message }, httpStatusCode, statusCode)
        {

        }

        public ErrorDataResult(string[] message, int httpStatusCode = 400, string statusCode = "400") : base(default, false, message, httpStatusCode, statusCode)
        {

        }

        public ErrorDataResult() : base(default, false)
        {

        }
    }
}
