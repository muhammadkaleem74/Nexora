using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(AdmissionAssessment))]
public class AdmissionAssessmentDto : EntityDto<long>
{
    public long ApplicationId { get; set; }

    public AssessmentType AssessmentType { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public long? ConductedByUserId { get; set; }

    public decimal? Score { get; set; }

    public decimal? MaxScore { get; set; }

    public string Remarks { get; set; }

    public AssessmentResult Result { get; set; }
}
