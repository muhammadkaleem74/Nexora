using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Nexora.Domain.Admissions.Enums;

namespace Nexora.Admissions.Dto;

public class PagedStudentRequestDto : PagedResultRequestDto, IShouldNormalize
{
    public string Keyword { get; set; }

    public StudentStatus? Status { get; set; }

    public long? CampusId { get; set; }

    public long? CurrentGradeLevelId { get; set; }

    public long? CurrentSectionId { get; set; }

    public string Sorting { get; set; }

    public void Normalize()
    {
        Keyword = Keyword?.Trim();

        if (string.IsNullOrWhiteSpace(Sorting))
            Sorting = "FirstName, LastName";
    }
}
