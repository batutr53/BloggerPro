using System.ComponentModel.DataAnnotations;

namespace BloggerPro.Application.DTOs.Contact;

public class ContactCreateDto
{
    [Required(ErrorMessage = "İsim alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "İsim en fazla 100 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [StringLength(255, ErrorMessage = "Email en fazla 255 karakter olabilir.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Konu alanı zorunludur.")]
    [StringLength(200, ErrorMessage = "Konu en fazla 200 karakter olabilir.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
    [StringLength(2000, ErrorMessage = "Mesaj en fazla 2000 karakter olabilir.")]
    public string Message { get; set; } = string.Empty;
}