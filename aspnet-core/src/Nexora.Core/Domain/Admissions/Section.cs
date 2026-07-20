using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Nexora.Domain.Admissions;

public class Section : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    public long GradeLevelId { get; set; }

    [Required]
    [StringLength(20)]
    public string Name { get; set; }

    public int Capacity { get; set; }

    public virtual GradeLevel GradeLevel { get; set; }
}
