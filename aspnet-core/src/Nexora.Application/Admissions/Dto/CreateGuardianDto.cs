using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Nexora.Domain.Admissions;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

[AutoMapTo(typeof(Guardian))]
public class CreateGuardianDto
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; }

    public GuardianRelationship Relationship { get; set; }

    [StringLength(50)]
    public string NationalIdNumber { get; set; }

    [StringLength(150)]
    public string Email { get; set; }

    [StringLength(30)]
    public string Phone { get; set; }

    [StringLength(100)]
    public string Occupation { get; set; }

    [StringLength(300)]
    public string Address { get; set; }
}
