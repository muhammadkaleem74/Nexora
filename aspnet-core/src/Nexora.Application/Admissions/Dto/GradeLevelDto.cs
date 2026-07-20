using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(GradeLevel))]
[AutoMapTo(typeof(GradeLevel))]
public class GradeLevelDto : EntityDto<long>
{
    public long CampusId { get; set; }

    public string CampusName { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(20)]
    public string Code { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}
