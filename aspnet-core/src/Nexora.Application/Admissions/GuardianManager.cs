using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using Nexora.Admissions.Dto;
using Nexora.Domain.Admissions;

namespace Nexora.Admissions;

/// <summary>
/// Shared application-layer helper: find or create a Guardian, avoiding duplicates
/// based on NationalIdNumber within the current tenant.
/// </summary>
public class GuardianManager : ITransientDependency
{
    private readonly IRepository<Guardian, long> _guardianRepository;

    public GuardianManager(IRepository<Guardian, long> guardianRepository)
    {
        _guardianRepository = guardianRepository;
    }

    /// <summary>
    /// If input carries a non-empty NationalIdNumber that matches an existing Guardian
    /// (ABP automatically scopes the query to the current tenant), returns that Guardian.
    /// Otherwise inserts a new Guardian and returns it.
    /// </summary>
    public async Task<Guardian> FindOrCreateAsync(CreateGuardianDto input)
    {
        if (!input.NationalIdNumber.IsNullOrWhiteSpace())
        {
            var existing = await _guardianRepository.GetAll()
                .FirstOrDefaultAsync(g => g.NationalIdNumber == input.NationalIdNumber);

            if (existing != null)
                return existing;
        }

        var guardian = new Guardian
        {
            FullName = input.FullName,
            Relationship = input.Relationship,
            NationalIdNumber = input.NationalIdNumber,
            Email = input.Email,
            Phone = input.Phone,
            Occupation = input.Occupation,
            Address = input.Address,
        };

        await _guardianRepository.InsertAndGetIdAsync(guardian);
        return guardian;
    }
}
