using System;
using System.ComponentModel.DataAnnotations;

namespace BloggerPro.Application.DTOs.User
{
    public class UpdateUserProfileDto
    {
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
        public string? LastName { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters")]
        public string? Bio { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL for the profile picture")]
        public string? ProfileImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot be longer than 100 characters")]
        public string? Location { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? Website { get; set; }

        [Url(ErrorMessage = "Please enter a valid Facebook URL")]
        public string? FacebookUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid Twitter URL")]
        public string? TwitterUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid Instagram URL")]
        public string? InstagramUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid LinkedIn URL")]
        public string? LinkedInUrl { get; set; }
    }
}
