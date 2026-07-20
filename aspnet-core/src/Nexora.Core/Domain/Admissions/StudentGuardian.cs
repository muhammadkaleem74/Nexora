using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Nexora.Domain.Admissions;

public class StudentGuardian : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    public long StudentId { get; set; }

    public long GuardianId { get; set; }

    public bool IsPrimaryContact { get; set; }

    public bool CanPickupStudent { get; set; }

    public virtual Student Student { get; set; }

    public virtual Guardian Guardian { get; set; }
}
