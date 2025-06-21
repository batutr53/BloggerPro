using BloggerPro.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace BloggerPro.Application.DTOs.PostModule
{
    public class UpdatePostModuleDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public PostModuleType Type { get; set; }
        
        [StringLength(4000)]
        public string? Content { get; set; }
        
        [StringLength(1000)]
        public string? MediaUrl { get; set; }
        
        [StringLength(50)]
        public string? Alignment { get; set; } = "left";
        
        [StringLength(50)]
        public string? Width { get; set; } = "100%";
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Order { get; set; }
    }
}
