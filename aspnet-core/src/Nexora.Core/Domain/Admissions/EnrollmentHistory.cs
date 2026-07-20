using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Domain.Admissions;

public class EnrollmentHistory : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    public long StudentId { get; set; }

    public long AcademicYearId { get; set; }

    public long GradeLevelId { get; set; }

    public long? SectionId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public PromotionStatus PromotionStatus { get; set; }

    [StringLength(300)]
    public string Remarks { get; set; }

    public virtual Student Student { get; set; }

    public virtual AcademicYear AcademicYear { get; set; }

    public virtual GradeLevel GradeLevel { get; set; }

    public virtual Section Section { get; set; }
}
