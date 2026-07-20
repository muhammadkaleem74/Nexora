using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class StudentGuardianDto : ApplicationGuardianDto
{
    public bool CanPickupStudent { get; set; }
}
