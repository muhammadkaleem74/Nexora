using System.ComponentModel.DataAnnotations;

namespace Nexora.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}