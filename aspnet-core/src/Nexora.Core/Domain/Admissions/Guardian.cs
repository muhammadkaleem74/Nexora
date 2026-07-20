using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Domain.Admissions;

public class Guardian : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    [Required]
    [StringLength(150)]
    public string FullName { get; set; }

    public GuardianRelationship Relationship { get; set; }

    [StringLength(50)]
    public string NationalIdNumber { get; set; }

    [StringLength(150)]
    public string Email { get; set; }

    [StringLength(30)]
    public string Phone { get; set; }

    [StringLength(100)]
    public string Occupation { get; set; }

    [StringLength(300)]
    public string Address { get; set; }
}
