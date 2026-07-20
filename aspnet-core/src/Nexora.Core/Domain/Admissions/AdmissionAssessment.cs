using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Domain.Admissions;

public class AdmissionAssessment : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    public long ApplicationId { get; set; }

    public AssessmentType AssessmentType { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public long? ConductedByUserId { get; set; }

    public decimal? Score { get; set; }

    public decimal? MaxScore { get; set; }

    [StringLength(1000)]
    public string Remarks { get; set; }

    public AssessmentResult Result { get; set; }

    public virtual AdmissionApplication Application { get; set; }
}
