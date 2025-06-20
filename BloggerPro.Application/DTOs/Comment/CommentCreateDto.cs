namespace BloggerPro.Application.DTOs.Comment
{
    public class CommentCreateDto
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = null!;
        public Guid? ParentCommentId { get; set; } 
    }

}
