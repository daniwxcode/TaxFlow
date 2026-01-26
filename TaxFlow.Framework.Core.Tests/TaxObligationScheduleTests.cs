using System;
using System.Collections.Generic;
using System.Linq;

using Core.Domain.Contracts.Validation;
using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;

using Xunit;

namespace TaxFlow.Framework.Core.Tests;

/// <summary>
/// Tests for the TaxObligationSchedule class, validating schedule creation, deadline management, and validation logic.
/// </summary>
/// <remarks>
/// This test class covers all aspects of tax obligation scheduling including declaration deadlines, payment deadlines,
/// validation rules, and schedule queries. Tests ensure proper error handling and business rule enforcement.
/// </remarks>
public class TaxObligationScheduleTests
{
    /// <summary>
    /// Verifies that creating a new schedule returns an empty schedule with no deadlines.
    /// </summary>
    /// <remarks>
    /// This test validates the initial state of a newly created tax obligation schedule,
    /// ensuring it has no declaration or payment deadlines configured.
    /// </remarks>
    [Fact]
    public void Create_Returns_Empty_Schedule()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();

        Assert.False(schedule.HasDeclarationDeadline);
        Assert.False(schedule.HasPaymentDeadlines);
        Assert.Equal(0, schedule.InstallmentCount);
    }

    /// <summary>
    /// Verifies that setting a declaration deadline properly configures the schedule.
    /// </summary>
    /// <remarks>
    /// This test ensures that the WithDeclarationDeadline method correctly sets the declaration deadline
    /// and updates the schedule's HasDeclarationDeadline flag accordingly.
    /// </remarks>
    [Fact]
    public void WithDeclarationDeadline_Sets_Declaration()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        DeclarationDeadline deadline = DeclarationDeadline.Create("DECL", "Annual Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero));

        schedule.WithDeclarationDeadline(deadline);

        Assert.True(schedule.HasDeclarationDeadline);
        Assert.Equal("DECL", schedule.DeclarationDeadline!.Key);
    }

    /// <summary>
    /// Verifies that multiple declaration deadlines can be added to a schedule.
    /// </summary>
    /// <remarks>
    /// This test validates the ability to configure multiple declaration deadlines (e.g., quarterly declarations)
    /// and ensures proper counting and multiple declaration flag management.
    /// </remarks>
    [Fact]
    public void AddDeclarationDeadline_Supports_Multiple_Declarations()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        DeclarationDeadline decl1 = DeclarationDeadline.Create(
            "DECL_Q1",
            "Q1 Declaration",
            new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero),
            DeadlinePeriodicity.Quarterly,
            TaxRegime.General,
            Duration.Zero,
            order: 1);
        DeclarationDeadline decl2 = DeclarationDeadline.Create(
            "DECL_Q2",
            "Q2 Declaration",
            new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero),
            DeadlinePeriodicity.Quarterly,
            TaxRegime.General,
            Duration.Zero,
            order: 2);

        schedule.AddDeclarationDeadline(decl1).AddDeclarationDeadline(decl2);

        Assert.True(schedule.HasMultipleDeclarations);
        Assert.Equal(2, schedule.DeclarationCount);
    }

    /// <summary>
    /// Verifies that payment deadlines are properly added to the schedule collection.
    /// </summary>
    /// <remarks>
    /// This test ensures that payment deadlines are correctly stored and that the installment count
    /// is properly maintained when adding multiple payment deadlines.
    /// </remarks>
    [Fact]
    public void AddPaymentDeadline_Adds_To_Collection()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        PaymentDeadline payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1);
        PaymentDeadline payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.AddPaymentDeadline(payment1).AddPaymentDeadline(payment2);

        Assert.True(schedule.HasPaymentDeadlines);
        Assert.Equal(2, schedule.InstallmentCount);
    }

    /// <summary>
    /// Verifies that adding duplicate payment deadline keys throws an InvalidOperationException.
    /// </summary>
    /// <remarks>
    /// This test ensures that the system properly enforces uniqueness of payment deadline keys
    /// and provides appropriate error handling for duplicate entries.
    /// </remarks>
    [Fact]
    public void AddPaymentDeadline_Throws_On_Duplicate_Key()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        PaymentDeadline payment1 = PaymentDeadline.Create("PAY", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero));
        PaymentDeadline payment2 = PaymentDeadline.Create("PAY", "Duplicate Payment", new DateTimeOffset(2025, 5, 31, 0, 0, 0, TimeSpan.Zero));

        schedule.AddPaymentDeadline(payment1);

        Assert.Throws<InvalidOperationException>(() => schedule.AddPaymentDeadline(payment2));
    }

    /// <summary>
    /// Verifies that validation detects when payment fractions exceed 100%.
    /// </summary>
    /// <remarks>
    /// This test ensures that the validation logic properly identifies when the sum of all payment deadline
    /// fractions exceeds 1.0 (100%) and returns appropriate validation errors.
    /// </remarks>
    [Fact]
    public void Validate_Detects_Fraction_Exceeds_100Percent()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        PaymentDeadline payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.6m, 1);
        PaymentDeadline payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.AddPaymentDeadline(payment1).AddPaymentDeadline(payment2);
        ValidationResult result = schedule.Validate();

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Code == "INVALID_FRACTION_TOTAL");
    }

    /// <summary>
    /// Verifies that validation detects when a declaration deadline is set after its linked payment deadline.
    /// </summary>
    /// <remarks>
    /// This test ensures that business rules are enforced where declaration deadlines must occur
    /// before their associated payment deadlines to maintain logical chronological order.
    /// </remarks>
    [Fact]
    public void Validate_Detects_Declaration_After_LinkedPayment()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        DeclarationDeadline declaration = DeclarationDeadline.Create("DECL", "Late Declaration", new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero));
        PaymentDeadline payment = PaymentDeadline.Create("PAY", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero))
            .LinkedToDeclaration("DECL");

        schedule.WithDeclarationDeadline(declaration).AddPaymentDeadline(payment);
        ValidationResult result = schedule.Validate();

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Code == "DECLARATION_AFTER_PAYMENT");
    }

    /// <summary>
    /// Verifies that validation detects when a payment deadline references a non-existent declaration.
    /// </summary>
    /// <remarks>
    /// This test ensures that referential integrity is maintained between payment deadlines and their
    /// linked declaration deadlines, preventing orphaned references.
    /// </remarks>
    [Fact]
    public void Validate_Detects_Invalid_Linked_Declaration()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        PaymentDeadline payment = PaymentDeadline.Create("PAY", "Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero))
            .LinkedToDeclaration("NON_EXISTENT");

        schedule.AddPaymentDeadline(payment);
        ValidationResult result = schedule.Validate();

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Code == "INVALID_LINKED_DECLARATION");
    }

    /// <summary>
    /// Verifies that validation detects duplicate payment order numbers.
    /// </summary>
    /// <remarks>
    /// This test ensures that the system enforces unique order numbers for payment deadlines
    /// to maintain proper sequencing and avoid conflicts in payment processing.
    /// </remarks>
    [Fact]
    public void Validate_Detects_Duplicate_Orders()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        PaymentDeadline payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1);
        PaymentDeadline payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 1); // Same order

        schedule.AddPaymentDeadline(payment1).AddPaymentDeadline(payment2);
        ValidationResult result = schedule.Validate();

        Assert.True(result.HasErrors);
        Assert.Contains(result.Errors, e => e.Code == "DUPLICATE_PAYMENT_ORDER");
    }

    /// <summary>
    /// Verifies that validation succeeds for a properly configured schedule.
    /// </summary>
    /// <remarks>
    /// This test validates the positive case where a tax obligation schedule with correct
    /// declaration and payment deadlines passes all validation rules successfully.
    /// </remarks>
    [Fact]
    public void Validate_Succeeds_For_Valid_Schedule()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        DeclarationDeadline declaration = DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero));
        PaymentDeadline payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1);
        PaymentDeadline payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.WithDeclarationDeadline(declaration)
                .AddPaymentDeadline(payment1)
                .AddPaymentDeadline(payment2);

        ValidationResult result = schedule.Validate();

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifies that validation succeeds for a schedule with properly linked payments to declarations.
    /// </summary>
    /// <remarks>
    /// This test validates scenarios where payment deadlines are explicitly linked to declaration deadlines
    /// and ensures that proper linkage does not cause validation failures.
    /// </remarks>
    [Fact]
    public void Validate_Succeeds_For_Valid_Schedule_With_Linked_Payments()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        DeclarationDeadline declaration = DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero));
        PaymentDeadline payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1)
            .LinkedToDeclaration("DECL");
        PaymentDeadline payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2)
            .LinkedToDeclaration("DECL");

        schedule.WithDeclarationDeadline(declaration)
                .AddPaymentDeadline(payment1)
                .AddPaymentDeadline(payment2);

        ValidationResult result = schedule.Validate();

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifies that the GetOverdueDeadlines method returns only deadlines that are past due.
    /// </summary>
    /// <remarks>
    /// This test ensures that the overdue deadline query correctly filters deadlines based on the
    /// provided assessment date and only returns those that have passed their due dates.
    /// </remarks>
    [Fact]
    public void GetOverdueDeadlines_Returns_Only_Overdue()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        DeclarationDeadline declaration = DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero));
        PaymentDeadline payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1);
        PaymentDeadline payment2 = PaymentDeadline.Create("PAY2", "Second Payment", new DateTimeOffset(2025, 7, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.WithDeclarationDeadline(declaration)
                .AddPaymentDeadline(payment1)
                .AddPaymentDeadline(payment2);

        DateTimeOffset asOf = new DateTimeOffset(2025, 5, 15, 0, 0, 0, TimeSpan.Zero);
        IReadOnlyList<TaxDeadline> overdue = schedule.GetOverdueDeadlines(asOf);

        Assert.Equal(2, overdue.Count); // Declaration and first payment are overdue
        Assert.Contains(overdue, d => d.Key == "DECL");
        Assert.Contains(overdue, d => d.Key == "PAY1");
        Assert.DoesNotContain(overdue, d => d.Key == "PAY2");
    }

    /// <summary>
    /// Verifies that payment deadlines correctly calculate the amount due based on their fraction.
    /// </summary>
    /// <remarks>
    /// This test ensures that the GetAmountDue method properly multiplies the total tax amount
    /// by the payment deadline's fraction to determine the specific payment amount.
    /// </remarks>
    [Fact]
    public void PaymentDeadline_GetAmountDue_Calculates_Fraction()
    {
        PaymentDeadline payment = PaymentDeadline.Create("PAY", "Payment", DateTimeOffset.Now, 0.25m, 1);

        decimal amountDue = payment.GetAmountDue(100000m);

        Assert.Equal(25000m, amountDue);
    }

    /// <summary>
    /// Verifies that penalty definitions can be attached to declaration deadlines.
    /// </summary>
    /// <remarks>
    /// This test ensures that the WithPenalty method properly associates penalty rules
    /// with declaration deadlines for enforcement of late filing penalties.
    /// </remarks>
    [Fact]
    public void DeclarationDeadline_WithPenalty_Attaches_Definition()
    {
        DeclarationDeadline deadline = DeclarationDeadline.Create("DECL", "Declaration", DateTimeOffset.Now)
            .WithPenalty(new PenaltyDefinition
            {
                Type = PenaltyType.Assiette,
                FixedAmount = 100m,
                AnnualRate = 0.12m
            });

        Assert.NotNull(deadline.PenaltyDefinition);
        Assert.Equal(100m, deadline.PenaltyDefinition.FixedAmount);
    }

    /// <summary>
    /// Verifies that penalty definitions can be attached to payment deadlines.
    /// </summary>
    /// <remarks>
    /// This test ensures that the WithPenalty method properly associates penalty rules
    /// with payment deadlines for enforcement of late payment penalties.
    /// </remarks>
    [Fact]
    public void PaymentDeadline_WithPenalty_Attaches_Definition()
    {
        PaymentDeadline deadline = PaymentDeadline.Create("PAY", "Payment", DateTimeOffset.Now)
            .WithPenalty(new PenaltyDefinition
            {
                Type = PenaltyType.Recouvrement,
                AnnualRate = 0.10m
            });

        Assert.NotNull(deadline.PenaltyDefinition);
        Assert.Equal(0.10m, deadline.PenaltyDefinition.AnnualRate);
    }

    /// <summary>
    /// Verifies that the method returns only payment deadlines linked to a specific declaration.
    /// </summary>
    /// <remarks>
    /// This test ensures that the GetPaymentsForDeclaration query correctly filters payment deadlines
    /// based on their declared linkage to specific declaration deadlines.
    /// </remarks>
    [Fact]
    public void GetPaymentsForDeclaration_Returns_Linked_Payments()
    {
        TaxObligationSchedule schedule = TaxObligationSchedule.Create();
        DeclarationDeadline declaration = DeclarationDeadline.Create("DECL", "Declaration", new DateTimeOffset(2025, 3, 31, 0, 0, 0, TimeSpan.Zero));
        PaymentDeadline payment1 = PaymentDeadline.Create("PAY1", "First Payment", new DateTimeOffset(2025, 4, 30, 0, 0, 0, TimeSpan.Zero), 0.5m, 1)
            .LinkedToDeclaration("DECL");
        PaymentDeadline payment2 = PaymentDeadline.Create("PAY2", "Unlinked Payment", new DateTimeOffset(2025, 5, 31, 0, 0, 0, TimeSpan.Zero), 0.5m, 2);

        schedule.WithDeclarationDeadline(declaration)
                .AddPaymentDeadline(payment1)
                .AddPaymentDeadline(payment2);

        IReadOnlyList<PaymentDeadline> linkedPayments = schedule.GetPaymentsForDeclaration("DECL");

        Assert.Single(linkedPayments);
        Assert.Equal("PAY1", linkedPayments[0].Key);
    }

    /// <summary>
    /// Verifies that legal references can be added to declaration deadlines for regulatory compliance.
    /// </summary>
    /// <remarks>
    /// This test ensures that declaration deadlines can be properly documented with their legal basis
    /// by adding references to relevant tax code articles and regulations.
    /// </remarks>
    [Fact]
    public void LegalReference_Can_Be_Added_To_Deadline()
    {
        DeclarationDeadline declaration = DeclarationDeadline.Create("DECL", "Declaration", DateTimeOffset.Now)
            .AddLegalReference(LegalReference.Create(
                LegalTextType.TaxCode,
                "CGI Art. 123",
                "Obligation de déclaration annuelle",
                "123"));

        Assert.True(declaration.HasLegalBasis);
        Assert.Single(declaration.LegalReferences);
    }

    /// <summary>
    /// Verifies that recurring deadlines correctly calculate their next occurrence date.
    /// </summary>
    /// <remarks>
    /// This test ensures that the GetNextOccurrence method properly calculates the next deadline date
    /// for recurring obligations like monthly or quarterly declarations based on their periodicity.
    /// </remarks>
    [Fact]
    public void Deadline_GetNextOccurrence_Works_For_Recurring()
    {
        DeclarationDeadline declaration = DeclarationDeadline.Create(
            "DECL_M",
            "Monthly Declaration",
            new DateTimeOffset(2025, 1, 15, 0, 0, 0, TimeSpan.Zero),
            DeadlinePeriodicity.Monthly);

        DateTimeOffset? next = declaration.GetNextOccurrence(new DateTimeOffset(2025, 3, 15, 0, 0, 0, TimeSpan.Zero));

        Assert.NotNull(next);
        // Jan 15 + 1 month = Feb 15, + 1 month = Mar 15, + 1 month = Apr 15
        Assert.Equal(new DateTimeOffset(2025, 4, 15, 0, 0, 0, TimeSpan.Zero), next!.Value);
    }

    /// <summary>
    /// Verifies that creating advance payment deadlines properly sets the payment type.
    /// </summary>
    /// <remarks>
    /// This test ensures that the CreateAdvance factory method correctly configures payment deadlines
    /// as advance payments and sets the appropriate payment type and flags.
    /// </remarks>
    [Fact]
    public void PaymentDeadline_CreateAdvance_Sets_PaymentType()
    {
        PaymentDeadline advance = PaymentDeadline.CreateAdvance("ADV", "Advance Payment", DateTimeOffset.Now, 0.25m, 1);

        Assert.Equal(PaymentType.Advance, advance.PaymentType);
        Assert.True(advance.IsAdvancePayment);
    }

    /// <summary>
    /// Verifies that creating balance payment deadlines properly sets the payment type.
    /// </summary>
    /// <remarks>
    /// This test ensures that the CreateBalance factory method correctly configures payment deadlines
    /// as balance payments and sets the appropriate payment type and flags for final tax settlements.
    /// </remarks>
    [Fact]
    public void PaymentDeadline_CreateBalance_Sets_PaymentType()
    {
        PaymentDeadline balance = PaymentDeadline.CreateBalance("BAL", "Balance Payment", DateTimeOffset.Now, 0.75m, 2);

        Assert.Equal(PaymentType.Balance, balance.PaymentType);
        Assert.True(balance.IsBalancePayment);
    }
}
