using System.ComponentModel.DataAnnotations;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class RecordAssessmentResultDto
{
    public decimal? Score { get; set; }

    public decimal? MaxScore { get; set; }

    [StringLength(1000)]
    public string Remarks { get; set; }

    [Required]
    public AssessmentResult Result { get; set; }
}
