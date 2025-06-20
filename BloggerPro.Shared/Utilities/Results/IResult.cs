namespace BloggerPro.Shared.Utilities.Results
{
    public interface IResult
    {
        bool Success { get; }
        string[] Message { get; }
        int HttpStatusCode { get; }
        string StatusCode { get; }
    }
}
