using System.Collections.Generic;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class AdmissionApplicationDetailDto : AdmissionApplicationListDto
{
    public string Nationality { get; set; }

    public string Religion { get; set; }

    public string PreviousSchoolName { get; set; }

    public string PreviousSchoolBoard { get; set; }

    public string PreviousGradeLevel { get; set; }

    public long? ReviewedByUserId { get; set; }

    public string ReviewNotes { get; set; }

    public string RejectionReason { get; set; }

    public List<ApplicationGuardianDto> Guardians { get; set; } = new();

    public List<ApplicationDocumentDto> Documents { get; set; } = new();

    public List<AdmissionAssessmentDto> Assessments { get; set; } = new();
}
