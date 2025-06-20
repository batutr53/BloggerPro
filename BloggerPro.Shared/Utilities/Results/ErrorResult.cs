namespace BloggerPro.Shared.Utilities.Results
{
    public class ErrorResult : Result
    {

        public ErrorResult(string message, int httpStatusCode = 400, string statusCode = "400") : base(false, message == null ? null : new[] { "400", message }, httpStatusCode, statusCode)
        {
        }

        public ErrorResult(string[] message, int httpStatusCode = 400, string statusCode = "400") : base(false, message, httpStatusCode, statusCode)
        {
        }

        public ErrorResult() : base(false, null, 400, "400")
        {
        }
    }
}
