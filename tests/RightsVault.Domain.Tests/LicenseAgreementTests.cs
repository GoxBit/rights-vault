using RightsVault.Domain.Entities;
using RightsVault.Domain.Exceptions;
using RightsVault.Domain.ValueObjects;

namespace RightsVault.Domain.Tests;

public class LicenseAgreementTests
{
    private static LicenseAgreement CreateAgreement(
        string title,
        int startYear,
        int startMonth,
        int startDay,
        int endYear,
        int endMonth,
        int endDay,
        bool exclusive = false) =>
        LicenseAgreement.Create(
            title,
            new DateRange(
                new DateOnly(startYear, startMonth, startDay),
                new DateOnly(endYear, endMonth, endDay)),
            exclusive);

    public class CreateTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_ThrowsArgumentException_WhenTitleIsNullOrWhitespace(string? title)
        {
            var term = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));

            Assert.ThrowsAny<ArgumentException>(() =>
                LicenseAgreement.Create(title!, term, hasExclusivity: false));
        }

        [Fact]
        public void Create_ThrowsArgumentNullException_WhenTermIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                LicenseAgreement.Create("Max Standard", null!, hasExclusivity: false));
        }

        [Fact]
        public void Create_AssignsUniqueId_AndPreservesProperties()
        {
            var term = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));

            var agreement = LicenseAgreement.Create("HBO Exclusive", term, hasExclusivity: true);

            Assert.NotEqual(Guid.Empty, agreement.Id);
            Assert.Equal("HBO Exclusive", agreement.Title);
            Assert.Equal(term, agreement.Term);
            Assert.True(agreement.HasExclusivity);
        }
    }

    public class RenewPreconditionsTests
    {
        [Fact]
        public void Renew_ThrowsArgumentNullException_WhenNewTermIsNull()
        {
            var target = CreateAgreement("Max Standard", 2026, 1, 1, 2026, 12, 31);

            Assert.Throws<ArgumentNullException>(() =>
                target.Renew(null!, []));
        }

        [Fact]
        public void Renew_ThrowsArgumentNullException_WhenExistingAgreementsIsNull()
        {
            var target = CreateAgreement("Max Standard", 2026, 1, 1, 2026, 12, 31);
            var newTerm = new DateRange(new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31));

            Assert.Throws<ArgumentNullException>(() =>
                target.Renew(newTerm, null!));
        }
    }

    public class RenewExclusivityTests
    {
        [Fact]
        public void Renew_ThrowsExclusivityConflict_WhenOverlapsExclusiveAgreement()
        {
            // Arrange
            var exclusive = CreateAgreement("HBO Exclusive", 2027, 1, 1, 2027, 12, 31, exclusive: true);
            var target = CreateAgreement("Max Non-Exclusive", 2026, 1, 1, 2026, 12, 31);
            var renewTerm = new DateRange(new DateOnly(2027, 6, 1), new DateOnly(2027, 12, 31));

            // Act
            var exception = Assert.Throws<ExclusivityConflictException>(() =>
                target.Renew(renewTerm, [exclusive]));

            // Assert
            Assert.Contains("Max Non-Exclusive", exception.Message);
        }

        [Fact]
        public void Renew_ThrowsExclusivityConflict_WhenMultipleExclusive_AndOneOverlaps()
        {
            var overlappingExclusive = CreateAgreement("Netflix Exclusive", 2027, 1, 1, 2027, 6, 30, exclusive: true);
            var nonOverlappingExclusive = CreateAgreement("Disney Exclusive", 2028, 1, 1, 2028, 12, 31, exclusive: true);
            var target = CreateAgreement("Max Standard", 2026, 1, 1, 2026, 12, 31);
            var renewTerm = new DateRange(new DateOnly(2027, 3, 1), new DateOnly(2027, 9, 30));

            Assert.Throws<ExclusivityConflictException>(() =>
                target.Renew(renewTerm, [overlappingExclusive, nonOverlappingExclusive]));
        }

        [Fact]
        public void Renew_UpdatesTerm_WhenNoExclusivityConflict()
        {
            var target = CreateAgreement("Max Standard", 2026, 1, 1, 2026, 12, 31);
            var newTerm = new DateRange(new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31));

            target.Renew(newTerm, []);

            Assert.Equal(newTerm, target.Term);
        }

        [Fact]
        public void Renew_UpdatesTerm_WhenExclusiveExists_ButDoesNotOverlap()
        {
            var exclusive = CreateAgreement("HBO Exclusive", 2028, 1, 1, 2028, 12, 31, exclusive: true);
            var target = CreateAgreement("Max Standard", 2026, 1, 1, 2026, 12, 31);
            var newTerm = new DateRange(new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31));

            target.Renew(newTerm, [exclusive]);

            Assert.Equal(newTerm, target.Term);
        }

        [Fact]
        public void Renew_UpdatesTerm_WhenOverlappingAgreementsAreNotExclusive()
        {
            var overlappingNonExclusive = CreateAgreement("Paramount Standard", 2027, 1, 1, 2027, 6, 30);
            var target = CreateAgreement("Max Standard", 2026, 1, 1, 2026, 12, 31);
            var newTerm = new DateRange(new DateOnly(2027, 3, 1), new DateOnly(2027, 9, 30));

            target.Renew(newTerm, [overlappingNonExclusive]);

            Assert.Equal(newTerm, target.Term);
        }

        [Fact]
        public void Renew_DoesNotUpdateTerm_WhenExclusivityConflictOccurs()
        {
            var exclusive = CreateAgreement("HBO Exclusive", 2027, 1, 1, 2027, 12, 31, exclusive: true);
            var target = CreateAgreement("Max Non-Exclusive", 2026, 1, 1, 2026, 12, 31);
            var originalTerm = target.Term;
            var renewTerm = new DateRange(new DateOnly(2027, 6, 1), new DateOnly(2027, 12, 31));

            Assert.Throws<ExclusivityConflictException>(() =>
                target.Renew(renewTerm, [exclusive]));

            Assert.Equal(originalTerm, target.Term);
        }
    }

    public class RenewEdgeCaseTests
    {
        [Fact]
        public void Renew_Succeeds_WhenTargetIsExclusive_AndOnlySelfInList()
        {
            var target = CreateAgreement("HBO Exclusive", 2026, 1, 1, 2026, 12, 31, exclusive: true);
            var newTerm = new DateRange(new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31));

            target.Renew(newTerm, [target]);

            Assert.Equal(newTerm, target.Term);
        }

        [Fact]
        public void Renew_Succeeds_WhenAdjacentExclusiveTerm_DoesNotOverlap()
        {
            var exclusive = CreateAgreement("HBO Exclusive", 2026, 1, 1, 2026, 6, 30, exclusive: true);
            var target = CreateAgreement("Max Standard", 2025, 1, 1, 2025, 12, 31);
            var newTerm = new DateRange(new DateOnly(2026, 6, 30), new DateOnly(2026, 12, 31));

            target.Renew(newTerm, [exclusive]);

            Assert.Equal(newTerm, target.Term);
        }
    }

    public class RenewOutOfScopeBehaviorTests
    {
        [Fact]
        public void Renew_Succeeds_WhenNewTermIsBeforeCurrentTerm()
        {
            var target = CreateAgreement("Max Standard", 2027, 1, 1, 2027, 12, 31);
            var earlierTerm = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));

            target.Renew(earlierTerm, []);

            Assert.Equal(earlierTerm, target.Term);
        }

        [Fact]
        public void Renew_Succeeds_WhenNewTermIsIdenticalToCurrentTerm()
        {
            var currentTerm = new DateRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
            var target = CreateAgreement("Max Standard", 2026, 1, 1, 2026, 12, 31);

            target.Renew(currentTerm, []);

            Assert.Equal(currentTerm, target.Term);
        }
    }

    public class DomainExceptionTests
    {
        [Fact]
        public void ExclusivityConflictException_InheritsFromDomainException()
        {
            var exception = new ExclusivityConflictException("test message");

            Assert.IsAssignableFrom<DomainException>(exception);
        }

        [Fact]
        public void PipelineDetectaFallos_Rojo()
        {
            Assert.True(false, "Fallo intencional para validar el pipeline");
        }
    }


}
