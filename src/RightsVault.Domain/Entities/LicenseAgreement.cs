using RightsVault.Domain.Enums;
using RightsVault.Domain.Exceptions;
using RightsVault.Domain.ValueObjects;

namespace RightsVault.Domain.Entities;

// Status: Draft, Active, Expired, Revoked. Enum — sin comportamiento propio aún; Create() inicia en Active.
// Territory: agregado independiente (ARCHITECTURE.md). Hoy TerritoryCode (ISO alpha-2) es la referencia; Renew() no filtra por territorio.
// LicenseType: Exclusive, NonExclusive, Limited. Solo Exclusive genera conflicto en Renew(); HasExclusivity desaparece.
public sealed class LicenseAgreement
{
    public Guid Id { get; }
    public string Title { get; }
    public DateRange Term { get; private set; }
    public TerritoryCode Territory { get; }
    public LicenseType LicenseType { get; }
    public LicenseStatus Status { get; }

    private LicenseAgreement()
    {
        Title = null!;
        Term = null!;
        Territory = null!;
    }

    private LicenseAgreement(
        Guid id,
        string title,
        DateRange term,
        TerritoryCode territory,
        LicenseType licenseType,
        LicenseStatus status)
    {
        Id = id;
        Title = title;
        Term = term;
        Territory = territory;
        LicenseType = licenseType;
        Status = status;
    }

    public static LicenseAgreement Create(
        string title,
        DateRange term,
        TerritoryCode territory,
        LicenseType licenseType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(term);
        ArgumentNullException.ThrowIfNull(territory);

        return new LicenseAgreement(
            Guid.NewGuid(),
            title,
            term,
            territory,
            licenseType,
            LicenseStatus.Active);
    }

    public void Renew(
        DateRange newTerm,
        IEnumerable<LicenseAgreement> existingAgreements)
    {
        ArgumentNullException.ThrowIfNull(newTerm);
        ArgumentNullException.ThrowIfNull(existingAgreements);

        var exclusivityConflict = existingAgreements
            .Where(agreement => agreement.Id != Id && agreement.LicenseType == LicenseType.Exclusive)
            .Any(agreement => agreement.Term.OverlapsWith(newTerm));

        if (exclusivityConflict)
        {
            throw new ExclusivityConflictException(
                $"Cannot renew '{Title}': overlaps with an exclusive agreement.");
        }

        Term = newTerm;
    }
}
