using Abp.Application.Services.Dto;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class ApplicationGuardianDto : EntityDto<long>
{
    public long GuardianId { get; set; }

    public string FullName { get; set; }

    public GuardianRelationship Relationship { get; set; }

    public string NationalIdNumber { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string Occupation { get; set; }

    public bool IsPrimaryContact { get; set; }
}
