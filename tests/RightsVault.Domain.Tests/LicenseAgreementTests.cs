using RightsVault.Domain.Entities;
using RightsVault.Domain.Exceptions;
using RightsVault.Domain.ValueObjects;

namespace RightsVault.Domain.Tests;

public class LicenseAgreementTests
{
    [Fact]
    public void Renew_ThrowsExclusivityConflict_WhenOverlapsExclusiveAgreement()
    {
        // Arrange
        var exclusiveTerm = new DateRange(
            new DateOnly(2027, 1, 1),
            new DateOnly(2027, 12, 31));
        var exclusive = LicenseAgreement.Create(
            "HBO Exclusive",
            exclusiveTerm,
            hasExclusivity: true);
        var target = LicenseAgreement.Create(
            "Max Non-Exclusive",
            new DateRange(
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 12, 31)),
            hasExclusivity: false);
        var renewTerm = new DateRange(
            new DateOnly(2027, 6, 1),
            new DateOnly(2027, 12, 31));

        // Act
        Action act = () => target.Renew(renewTerm, [exclusive]);

        // Assert
        Assert.Throws<ExclusivityConflictException>(act);
    }

    [Fact]
    public void Renew_UpdatesTerm_WhenNoExclusivityConflict()
    {
        // Arrange
        var target = LicenseAgreement.Create(
            "Max Standard",
            new DateRange(
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 12, 31)),
            hasExclusivity: false);
        var newTerm = new DateRange(
            new DateOnly(2027, 1, 1),
            new DateOnly(2027, 12, 31));

        // Act
        target.Renew(newTerm, []);

        // Assert
        Assert.Equal(newTerm, target.Term);
    }
}
