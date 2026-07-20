using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace Nexora.Admissions.Dto;

public class PagedGradeLevelRequestDto : PagedResultRequestDto, IShouldNormalize
{
    public string Keyword { get; set; }

    public long? CampusId { get; set; }

    public bool? IsActive { get; set; }

    public string Sorting { get; set; }

    public void Normalize()
    {
        Keyword = Keyword?.Trim();

        if (string.IsNullOrWhiteSpace(Sorting))
            Sorting = "SortOrder, Name";
    }
}
