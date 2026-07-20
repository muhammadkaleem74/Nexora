using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Domain.Admissions;

public class Student : FullAuditedEntity<long>, IMustHaveTenant
{
    public int TenantId { get; set; }

    public long? ApplicationId { get; set; }

    [Required]
    [StringLength(30)]
    public string RegistrationNumber { get; set; }

    [StringLength(30)]
    public string GRNumber { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public GenderType Gender { get; set; }

    [StringLength(5)]
    public string BloodGroup { get; set; }

    [StringLength(100)]
    public string Nationality { get; set; }

    [StringLength(50)]
    public string Religion { get; set; }

    [StringLength(500)]
    public string PhotoUrl { get; set; }

    public long CampusId { get; set; }

    public long CurrentGradeLevelId { get; set; }

    public long? CurrentSectionId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public StudentStatus Status { get; set; }

    public virtual AdmissionApplication Application { get; set; }

    public virtual Campus Campus { get; set; }

    public virtual GradeLevel CurrentGradeLevel { get; set; }

    public virtual Section CurrentSection { get; set; }

    public virtual ICollection<StudentGuardian> StudentGuardians { get; set; }

    public virtual ICollection<EnrollmentHistory> EnrollmentHistories { get; set; }
}
