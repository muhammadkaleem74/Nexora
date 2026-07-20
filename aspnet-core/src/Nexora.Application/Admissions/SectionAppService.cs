using System.Linq;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Nexora.Admissions.Dto;
using Nexora.Domain.Admissions;
using System.Linq.Dynamic.Core;

namespace Nexora.Admissions;

[AbpAuthorize]
public class SectionAppService
    : AsyncCrudAppService<Section, SectionDto, long,
                          PagedSectionRequestDto,
                          SectionDto,
                          SectionDto>,
      ISectionAppService
{
    public SectionAppService(IRepository<Section, long> repository)
        : base(repository)
    {
    }

    protected override IQueryable<Section> CreateFilteredQuery(PagedSectionRequestDto input)
    {
        return Repository.GetAll()
            .Include(s => s.GradeLevel)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                s => s.Name.Contains(input.Keyword))
            .WhereIf(input.GradeLevelId.HasValue, s => s.GradeLevelId == input.GradeLevelId.Value);
    }

    protected override IQueryable<Section> ApplySorting(
        IQueryable<Section> query, PagedSectionRequestDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
            return query.OrderBy(s => s.Name);

        return query.OrderBy(input.Sorting);
    }
}
