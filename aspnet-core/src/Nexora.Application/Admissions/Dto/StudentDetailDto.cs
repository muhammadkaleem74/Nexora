using System.Collections.Generic;

namespace Nexora.Admissions.Dto;

public class StudentDetailDto : StudentListDto
{
    public long? ApplicationId { get; set; }

    public string BloodGroup { get; set; }

    public string Nationality { get; set; }

    public string Religion { get; set; }

    public string PhotoUrl { get; set; }

    public List<StudentGuardianDto> Guardians { get; set; } = new();

    public List<EnrollmentHistoryDto> EnrollmentHistories { get; set; } = new();
}
