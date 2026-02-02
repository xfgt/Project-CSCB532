using System;
using Xunit;

public class CreditModelTests
{
    [Fact]
    public void OnPost_AnnuityWithPromo_ComputesExpectedTotals()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 10000,
            Months = 12,
            InterestRate = 12,
            PaymentType = "annuity",
            PromoMonths = 2,
            PromoRate = 6,
            GraceMonths = 0
        };

        var effectiveRate = model.InterestRate / 100.0 / 12.0;
        var promoRate = model.PromoRate / 100.0 / 12.0;
        var mainPeriod = model.Months - model.PromoMonths - model.GraceMonths;
        var expectedPromoPayment = model.Amount * promoRate / (1 - Math.Pow(1 + promoRate, -model.PromoMonths));
        var expectedNormalPayment = model.Amount * effectiveRate / (1 - Math.Pow(1 + effectiveRate, -mainPeriod));
        var expectedTotalPaid = expectedPromoPayment * model.PromoMonths + expectedNormalPayment * mainPeriod;

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedNormalPayment, model.MonthlyPayment, 2);
        Assert.Equal(expectedTotalPaid, model.TotalPaid, 2);
        Assert.Equal(expectedTotalPaid - model.Amount, model.TotalInterest, 2);
    }

    [Fact]
    public void OnPost_AnnuityWithFees_IncludesAllFeesInTotals()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 5000,
            Months = 24,
            InterestRate = 10,
            PaymentType = "annuity",
            ApplicationFee = new FeeItem { Type = "percent", Value = 1 },
            AnnualFee = new FeeItem { Type = "fixed", Value = 120 }
        };

        var effectiveRate = model.InterestRate / 100.0 / 12.0;
        var expectedPayment = model.Amount * effectiveRate / (1 - Math.Pow(1 + effectiveRate, -model.Months));
        var expectedFees = model.Amount * (model.ApplicationFee.Value / 100.0) + model.AnnualFee.Value * (model.Months / 12.0);
        var expectedTotalPaid = expectedPayment * model.Months + expectedFees;

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedPayment, model.MonthlyPayment, 2);
        Assert.Equal(expectedTotalPaid, model.TotalPaid, 2);
        Assert.Equal(expectedTotalPaid - model.Amount, model.TotalInterest, 2);
    }

    [Fact]
    public void OnPost_LinearPayment_ComputesExpectedTotals()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 12000,
            Months = 12,
            InterestRate = 12,
            PaymentType = "linear"
        };

        var effectiveRate = model.InterestRate / 100.0 / 12.0;
        double expectedTotal = 0;
        double remaining = model.Amount;
        for (int i = 0; i < model.Months; i++)
        {
            double principal = model.Amount / model.Months;
            double interest = remaining * effectiveRate;
            expectedTotal += principal + interest;
            remaining -= principal;
        }
        var expectedMonthlyPayment = model.Amount / model.Months + (model.Amount * effectiveRate);

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedMonthlyPayment, model.MonthlyPayment, 2);
        Assert.Equal(expectedTotal, model.TotalPaid, 2);
        Assert.Equal(expectedTotal - model.Amount, model.TotalInterest, 2);
    }

    [Fact]
    public void OnPost_ZeroMonths_HandlesGracefully()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 5000,
            Months = 0,
            InterestRate = 10,
            PaymentType = "annuity"
        };

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(0, model.MonthlyPayment, 2);
        Assert.Equal(0, model.TotalPaid, 2);
        Assert.Equal(-model.Amount, model.TotalInterest, 2);
    }

    [Fact]
    public void OnPost_InvalidInput_WithZeroMonthsLinear_ProducesInfiniteMonthlyPayment()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 10000,
            Months = 0,
            InterestRate = 10,
            PaymentType = "linear"
        };

        // Act
        var exception = Record.Exception(() => model.OnPost());

        // Assert
        Assert.Null(exception);
        Assert.True(model.ShowResults);
        Assert.True(double.IsPositiveInfinity(model.MonthlyPayment));
        Assert.Equal(-model.Amount, model.TotalInterest, 2);
    }
}
