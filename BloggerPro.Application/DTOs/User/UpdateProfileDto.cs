namespace BloggerPro.Application.DTOs.User
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string Location { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Website { get; set; }

        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
    }
}
