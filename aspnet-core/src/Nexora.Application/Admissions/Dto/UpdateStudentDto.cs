using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

[AutoMapTo(typeof(Student))]
public class UpdateStudentDto : EntityDto<long>
{
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

    public StudentStatus Status { get; set; }
}
