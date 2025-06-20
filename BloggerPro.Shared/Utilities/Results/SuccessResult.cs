namespace BloggerPro.Shared.Utilities.Results
{
    public class SuccessResult : Result
    {
        public SuccessResult(string message, int httpStatusCode = 200, string statusCode = "200") : base(true, message == null ? null : new[] { message }, httpStatusCode, statusCode)
        {
        }

        public SuccessResult(string[] message, int httpStatusCode = 200, string statusCode = "200") : base(true, message, httpStatusCode, statusCode)
        {
        }

        public SuccessResult() : base(true, null, 200, "200")
        {
        }
    }
}
