using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Domain.Admissions;

public class AdmissionApplication : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    [Required]
    [StringLength(30)]
    public string ApplicationNumber { get; set; }

    public long AcademicYearId { get; set; }

    public long CampusId { get; set; }

    public long DesiredGradeLevelId { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public GenderType Gender { get; set; }

    [StringLength(100)]
    public string Nationality { get; set; }

    [StringLength(50)]
    public string Religion { get; set; }

    [StringLength(200)]
    public string PreviousSchoolName { get; set; }

    [StringLength(100)]
    public string PreviousSchoolBoard { get; set; }

    [StringLength(50)]
    public string PreviousGradeLevel { get; set; }

    public ApplicationStatus Status { get; set; }

    public DateTime SubmittedDate { get; set; }

    public long? ReviewedByUserId { get; set; }

    [StringLength(1000)]
    public string ReviewNotes { get; set; }

    [StringLength(500)]
    public string RejectionReason { get; set; }

    public virtual AcademicYear AcademicYear { get; set; }

    public virtual Campus Campus { get; set; }

    public virtual GradeLevel DesiredGradeLevel { get; set; }

    public virtual ICollection<ApplicationGuardian> ApplicationGuardians { get; set; }

    public virtual ICollection<ApplicationDocument> Documents { get; set; }

    public virtual ICollection<AdmissionAssessment> Assessments { get; set; }
}
