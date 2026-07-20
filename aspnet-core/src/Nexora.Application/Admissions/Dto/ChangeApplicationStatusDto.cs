using System.ComponentModel.DataAnnotations;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class ChangeApplicationStatusDto
{
    public long ApplicationId { get; set; }

    public ApplicationStatus NewStatus { get; set; }

    [StringLength(1000)]
    public string Notes { get; set; }

    [StringLength(500)]
    public string RejectionReason { get; set; }
}
