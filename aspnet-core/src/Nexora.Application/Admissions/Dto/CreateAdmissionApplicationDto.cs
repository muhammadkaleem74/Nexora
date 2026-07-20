using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

[AutoMapTo(typeof(AdmissionApplication))]
public class CreateAdmissionApplicationDto
{
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
}
