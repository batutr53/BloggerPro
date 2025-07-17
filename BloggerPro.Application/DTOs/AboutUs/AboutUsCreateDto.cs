using System.ComponentModel.DataAnnotations;

namespace BloggerPro.Application.DTOs.AboutUs;

public class AboutUsCreateDto
{
    [Required(ErrorMessage = "Başlık alanı zorunludur.")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik alanı zorunludur.")]
    [StringLength(5000, ErrorMessage = "İçerik en fazla 5000 karakter olabilir.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Misyon alanı zorunludur.")]
    [StringLength(1000, ErrorMessage = "Misyon en fazla 1000 karakter olabilir.")]
    public string Mission { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vizyon alanı zorunludur.")]
    [StringLength(1000, ErrorMessage = "Vizyon en fazla 1000 karakter olabilir.")]
    public string Vision { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;
}