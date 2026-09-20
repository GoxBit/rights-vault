using RightsVault.Domain.Repositories;
using RightsVault.Domain.ValueObjects;

namespace RightsVault.Application.UseCases.RenewLicenseAgreement;

public sealed class RenewLicenseAgreementHandler(
    ILicenseAgreementRepository repository)
{
    public async Task HandleAsync(
        RenewLicenseAgreementCommand command,
        CancellationToken ct = default)
    {
        var agreement = await repository.GetByIdAsync(command.AgreementId, ct)
            ?? throw new InvalidOperationException(
                   $"Agreement {command.AgreementId} not found.");

        var allAgreements = await repository.GetAllAsync(ct);
        var newTerm = new DateRange(command.NewStart, command.NewEnd);

        agreement.Renew(newTerm, allAgreements);

        await repository.SaveAsync(agreement, ct);
    }
}
