using System.Linq;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Nexora.Admissions.Dto;
using Nexora.Domain.Admissions;
using System.Linq.Dynamic.Core;

namespace Nexora.Admissions;

[AbpAuthorize]
public class CampusAppService
    : AsyncCrudAppService<Campus, CampusDto, long,
                          PagedCampusRequestDto,
                          CampusDto,
                          CampusDto>,
      ICampusAppService
{
    public CampusAppService(IRepository<Campus, long> repository)
        : base(repository)
    {
    }

    protected override IQueryable<Campus> CreateFilteredQuery(PagedCampusRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                c => c.Name.Contains(input.Keyword) ||
                     c.City.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue, c => c.IsActive == input.IsActive.Value);
    }

    protected override IQueryable<Campus> ApplySorting(
        IQueryable<Campus> query, PagedCampusRequestDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
            return query.OrderBy(c => c.Name);

        return query.OrderBy(input.Sorting);
    }
}
