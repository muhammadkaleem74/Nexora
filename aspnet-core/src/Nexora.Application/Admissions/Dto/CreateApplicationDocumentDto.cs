using System.ComponentModel.DataAnnotations;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class CreateApplicationDocumentDto
{
    public DocumentType DocumentType { get; set; }

    [Required]
    [StringLength(255)]
    public string FileName { get; set; }

    [StringLength(500)]
    public string FileUrl { get; set; }

    public int? FileSizeKb { get; set; }
}
