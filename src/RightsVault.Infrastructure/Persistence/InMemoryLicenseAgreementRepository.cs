using RightsVault.Domain.Entities;
using RightsVault.Domain.Repositories;

namespace RightsVault.Infrastructure.Persistence;

public sealed class InMemoryLicenseAgreementRepository
    : ILicenseAgreementRepository
{
    private readonly Dictionary<Guid, LicenseAgreement> _store = [];

    public Task<LicenseAgreement?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<IReadOnlyList<LicenseAgreement>> GetAllAsync(CancellationToken ct)
        => Task.FromResult<IReadOnlyList<LicenseAgreement>>([.. _store.Values]);

    public Task SaveAsync(LicenseAgreement agreement, CancellationToken ct)
    {
        _store[agreement.Id] = agreement;
        return Task.CompletedTask;
    }
}
