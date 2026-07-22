using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Nexora.Domain.Admissions;

public class ApplicationGuardian : FullAuditedEntity<long>, IMustHaveTenant, IHasPrimaryContact
{
    public int TenantId { get; set; }

    public long ApplicationId { get; set; }

    public long GuardianId { get; set; }

    public bool IsPrimaryContact { get; set; }

    public virtual AdmissionApplication Application { get; set; }

    public virtual Guardian Guardian { get; set; }
}
