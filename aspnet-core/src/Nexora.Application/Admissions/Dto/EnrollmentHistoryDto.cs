using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(EnrollmentHistory))]
public class EnrollmentHistoryDto : EntityDto<long>
{
    public long StudentId { get; set; }

    public long AcademicYearId { get; set; }

    public string AcademicYearName { get; set; }

    public long GradeLevelId { get; set; }

    public string GradeLevelName { get; set; }

    public long? SectionId { get; set; }

    public string SectionName { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public PromotionStatus PromotionStatus { get; set; }

    public string Remarks { get; set; }
}
