using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(AcademicYear))]
[AutoMapTo(typeof(AcademicYear))]
public class AcademicYearDto : EntityDto<long>
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}
