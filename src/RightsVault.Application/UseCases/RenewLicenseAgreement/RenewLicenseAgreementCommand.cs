namespace RightsVault.Application.UseCases.RenewLicenseAgreement;

public sealed record RenewLicenseAgreementCommand(
    Guid AgreementId,
    DateOnly NewStart,
    DateOnly NewEnd);
