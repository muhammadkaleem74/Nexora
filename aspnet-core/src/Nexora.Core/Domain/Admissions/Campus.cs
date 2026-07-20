using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Nexora.Domain.Admissions;

public class Campus : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    [StringLength(100)]
    public string City { get; set; }

    [StringLength(100)]
    public string Country { get; set; }

    [StringLength(300)]
    public string Address { get; set; }

    [StringLength(30)]
    public string ContactNumber { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<GradeLevel> GradeLevels { get; set; }
}
