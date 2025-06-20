namespace BloggerPro.Application.DTOs.PostRating;

public class PostRatingListDto
{
    public Guid Id { get; set; }
    public int RatingValue { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}
