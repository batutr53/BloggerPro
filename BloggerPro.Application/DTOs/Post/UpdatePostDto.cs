using System;
using System.Collections.Generic;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.DTOs.Post
{
    public class UpdatePostDto : BasePostDto
    {
        public Guid Id { get; set; }
        public List<PostModuleDto> Modules { get; set; } = new();
        public DateTime? PublishDate { get; set; }
    }
}
