using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Nexora.Domain.Admissions;

public class GradeLevel : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    public long CampusId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(20)]
    public string Code { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual Campus Campus { get; set; }

    public virtual ICollection<Section> Sections { get; set; }
}
