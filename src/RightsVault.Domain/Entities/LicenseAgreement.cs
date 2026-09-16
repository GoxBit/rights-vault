using RightsVault.Domain.Exceptions;
using RightsVault.Domain.ValueObjects;

namespace RightsVault.Domain.Entities;

public sealed class LicenseAgreement
{
    public Guid Id { get; }
    public string Title { get; }
    public DateRange Term { get; private set; }
    public bool HasExclusivity { get; }

    private LicenseAgreement()
    {
        Title = null!;
        Term = null!;
    }

    private LicenseAgreement(
        Guid id,
        string title,
        DateRange term,
        bool hasExclusivity)
    {
        Id = id;
        Title = title;
        Term = term;
        HasExclusivity = hasExclusivity;
    }

    public static LicenseAgreement Create(
        string title,
        DateRange term,
        bool hasExclusivity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(term);

        return new LicenseAgreement(
            Guid.NewGuid(),
            title,
            term,
            hasExclusivity);
    }

    public void Renew(
        DateRange newTerm,
        IEnumerable<LicenseAgreement> existingAgreements)
    {
        ArgumentNullException.ThrowIfNull(newTerm);
        ArgumentNullException.ThrowIfNull(existingAgreements);

        var exclusivityConflict = existingAgreements
            .Where(agreement => agreement.Id != Id && agreement.HasExclusivity)
            .Any(agreement => agreement.Term.OverlapsWith(newTerm));

        if (exclusivityConflict)
        {
            throw new ExclusivityConflictException(
                $"Cannot renew '{Title}': overlaps with an exclusive agreement.");
        }

        Term = newTerm;
    }
}
