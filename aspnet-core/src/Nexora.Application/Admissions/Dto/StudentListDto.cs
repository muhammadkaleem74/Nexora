using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(Student))]
public class StudentListDto : EntityDto<long>
{
    public string RegistrationNumber { get; set; }

    public string GRNumber { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public GenderType Gender { get; set; }

    public long CampusId { get; set; }

    public string CampusName { get; set; }

    public long CurrentGradeLevelId { get; set; }

    public string CurrentGradeLevelName { get; set; }

    public long? CurrentSectionId { get; set; }

    public string CurrentSectionName { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public StudentStatus Status { get; set; }
}
