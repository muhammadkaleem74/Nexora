using System.ComponentModel.DataAnnotations;

namespace Nexora.Configuration.Dto;

public class ChangeUiThemeInput
{
    [Required]
    [StringLength(32)]
    public string Theme { get; set; }
}
