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
public class GradeLevelAppService
    : AsyncCrudAppService<GradeLevel, GradeLevelDto, long,
                          PagedGradeLevelRequestDto,
                          GradeLevelDto,
                          GradeLevelDto>,
      IGradeLevelAppService
{
    public GradeLevelAppService(IRepository<GradeLevel, long> repository)
        : base(repository)
    {
    }

    protected override IQueryable<GradeLevel> CreateFilteredQuery(PagedGradeLevelRequestDto input)
    {
        return Repository.GetAll()
            .Include(g => g.Campus)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                g => g.Name.Contains(input.Keyword) ||
                     g.Code.Contains(input.Keyword))
            .WhereIf(input.CampusId.HasValue, g => g.CampusId == input.CampusId.Value)
            .WhereIf(input.IsActive.HasValue, g => g.IsActive == input.IsActive.Value);
    }

    protected override IQueryable<GradeLevel> ApplySorting(
        IQueryable<GradeLevel> query, PagedGradeLevelRequestDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
            return query.OrderBy(g => g.SortOrder).ThenBy(g => g.Name);

        return query.OrderBy(input.Sorting);
    }
}
