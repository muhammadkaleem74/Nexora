using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(Section))]
[AutoMapTo(typeof(Section))]
public class SectionDto : EntityDto<long>
{
    public long GradeLevelId { get; set; }

    public string GradeLevelName { get; set; }

    [Required]
    [StringLength(20)]
    public string Name { get; set; }

    public int Capacity { get; set; }
}
