using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Nexora.Domain.Admissions;

public class AcademicYear : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}
