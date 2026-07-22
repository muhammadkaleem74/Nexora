using System.ComponentModel.DataAnnotations;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class VerifyDocumentDto
{
    [Required]
    public VerificationStatus VerificationStatus { get; set; }

    [StringLength(300)]
    public string RejectionNote { get; set; }
}
