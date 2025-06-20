namespace BloggerPro.Application.DTOs.PostRating;

public class PostRatingCreateDto
{
    public Guid PostId { get; set; }
    public int RatingValue { get; set; } // 1–5
}
