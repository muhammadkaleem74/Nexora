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
public class AcademicYearAppService
    : AsyncCrudAppService<AcademicYear, AcademicYearDto, long,
                          PagedAcademicYearRequestDto,
                          AcademicYearDto,
                          AcademicYearDto>,
      IAcademicYearAppService
{
    public AcademicYearAppService(IRepository<AcademicYear, long> repository)
        : base(repository)
    {
    }

    protected override IQueryable<AcademicYear> CreateFilteredQuery(PagedAcademicYearRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                a => a.Name.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue, a => a.IsActive == input.IsActive.Value);
    }

    protected override IQueryable<AcademicYear> ApplySorting(
        IQueryable<AcademicYear> query, PagedAcademicYearRequestDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
            return query.OrderByDescending(a => a.StartDate);

        return query.OrderBy(input.Sorting);
    }
}
