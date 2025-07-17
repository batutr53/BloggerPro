using System.ComponentModel.DataAnnotations;

namespace BloggerPro.Application.DTOs.TeamMember;

public class TeamMemberCreateDto
{
    [Required(ErrorMessage = "İsim alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "İsim en fazla 100 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pozisyon alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Pozisyon en fazla 100 karakter olabilir.")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Departman alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Departman en fazla 100 karakter olabilir.")]
    public string Department { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Biyografi en fazla 1000 karakter olabilir.")]
    public string? Bio { get; set; }

    [StringLength(500, ErrorMessage = "Resim URL'si en fazla 500 karakter olabilir.")]
    public string? ImageUrl { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [StringLength(100, ErrorMessage = "Email en fazla 100 karakter olabilir.")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "LinkedIn URL'si en fazla 500 karakter olabilir.")]
    public string? LinkedInUrl { get; set; }

    [StringLength(500, ErrorMessage = "Twitter URL'si en fazla 500 karakter olabilir.")]
    public string? TwitterUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;
}