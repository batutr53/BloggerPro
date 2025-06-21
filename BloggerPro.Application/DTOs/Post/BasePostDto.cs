using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.DTOs.Post
{
    public abstract class BasePostDto
    {
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(200)]
        public string Slug { get; set; }

        [Required]
        [StringLength(500)]
        public string Excerpt { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(500)]
        public string FeaturedImage { get; set; }

        public bool AllowComments { get; set; } = true;
        public bool IsFeatured { get; set; }
        public PostStatus Status { get; set; } = PostStatus.Draft;
        public PostVisibility Visibility { get; set; } = PostVisibility.Public;
        public List<Guid> CategoryIds { get; set; } = new();
        public List<Guid> TagIds { get; set; } = new();
    }
}
