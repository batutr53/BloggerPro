using System.ComponentModel.DataAnnotations;

namespace BloggerPro.Application.DTOs.Footer;

public class FooterUpdateDto
{
    [Required(ErrorMessage = "ID zorunludur.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Bölüm başlığı zorunludur.")]
    [StringLength(100, ErrorMessage = "Bölüm başlığı en fazla 100 karakter olabilir.")]
    public string SectionTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik zorunludur.")]
    [StringLength(2000, ErrorMessage = "İçerik en fazla 2000 karakter olabilir.")]
    public string Content { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Link URL'si en fazla 500 karakter olabilir.")]
    public string? LinkUrl { get; set; }

    [StringLength(100, ErrorMessage = "Link metni en fazla 100 karakter olabilir.")]
    public string? LinkText { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Sıralama 0'dan büyük olmalıdır.")]
    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Footer tipi zorunludur.")]
    [StringLength(50, ErrorMessage = "Footer tipi en fazla 50 karakter olabilir.")]
    public string FooterType { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Icon class en fazla 100 karakter olabilir.")]
    public string? IconClass { get; set; }

    [StringLength(100, ErrorMessage = "Parent section en fazla 100 karakter olabilir.")]
    public string? ParentSection { get; set; }
}