using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(AdmissionApplication))]
public class AdmissionApplicationListDto : EntityDto<long>
{
    public string ApplicationNumber { get; set; }

    public long AcademicYearId { get; set; }

    public string AcademicYearName { get; set; }

    public long CampusId { get; set; }

    public string CampusName { get; set; }

    public long DesiredGradeLevelId { get; set; }

    public string DesiredGradeLevelName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public GenderType Gender { get; set; }

    public ApplicationStatus Status { get; set; }

    public DateTime SubmittedDate { get; set; }

    public DateTime CreationTime { get; set; }
}
