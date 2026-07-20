using System;
using System.ComponentModel.DataAnnotations;

namespace Nexora.Admissions.Dto;

public class ConvertToStudentDto
{
    public long ApplicationId { get; set; }

    [Required]
    [StringLength(30)]
    public string RegistrationNumber { get; set; }

    [Required]
    [StringLength(30)]
    public string GRNumber { get; set; }

    public long CampusId { get; set; }

    public long CurrentGradeLevelId { get; set; }

    public long? CurrentSectionId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public long AcademicYearId { get; set; }
}
