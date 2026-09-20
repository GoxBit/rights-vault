using RightsVault.Domain.Entities;

namespace RightsVault.Domain.Repositories;

public interface ILicenseAgreementRepository
{
    Task<LicenseAgreement?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LicenseAgreement>> GetAllAsync(CancellationToken ct = default);
    Task SaveAsync(LicenseAgreement agreement, CancellationToken ct = default);
}
