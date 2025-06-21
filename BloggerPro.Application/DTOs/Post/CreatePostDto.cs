using System;
using System.Collections.Generic;
using BloggerPro.Application.DTOs.PostModule;
using BloggerPro.Domain.Enums;

namespace BloggerPro.Application.DTOs.Post
{
    public class CreatePostDto : BasePostDto
    {
        public List<PostModuleDto> Modules { get; set; } = new();
    }
}
