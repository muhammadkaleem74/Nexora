using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;

namespace Nexora.Admissions.Dto;

[AutoMapFrom(typeof(Campus))]
[AutoMapTo(typeof(Campus))]
public class CampusDto : EntityDto<long>
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; }

    [StringLength(100)]
    public string City { get; set; }

    [StringLength(100)]
    public string Country { get; set; }

    [StringLength(300)]
    public string Address { get; set; }

    [StringLength(30)]
    public string ContactNumber { get; set; }

    public bool IsActive { get; set; }
}
