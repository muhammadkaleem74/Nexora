using System;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class CreateAssessmentDto
{
    public AssessmentType AssessmentType { get; set; }

    public DateTime? ScheduledDate { get; set; }
}
