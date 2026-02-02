using System;
using Xunit;

namespace Project_CSCB532.Tests.Pages;

public class CreditModelTests
{
    [Fact]
    public void OnPost_Annuity_NoPromo_ComputesTotals()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 10000,
            Months = 12,
            InterestRate = 12,
            PaymentType = "annuity"
        };

        var effectiveRate = model.InterestRate / 100.0 / 12.0;
        var mainPeriod = model.Months - model.PromoMonths - model.GraceMonths;
        var expectedPayment = model.Amount * effectiveRate / (1 - Math.Pow(1 + effectiveRate, -mainPeriod));
        var expectedTotalPaid = expectedPayment * mainPeriod;

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedPayment, model.MonthlyPayment, 2);
        Assert.Equal(expectedTotalPaid, model.TotalPaid, 2);
        Assert.Equal(expectedTotalPaid - model.Amount, model.TotalInterest, 2);
    }

    [Fact]
    public void OnPost_Annuity_WithPromoAndFees_ComputesTotals()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 8000,
            Months = 24,
            InterestRate = 10,
            PaymentType = "annuity",
            PromoMonths = 6,
            PromoRate = 5,
            GraceMonths = 0,
            ApplicationFee = new FeeItem { Type = "percent", Value = 1.5 },
            ProcessingFee = new FeeItem { Type = "fixed", Value = 50 },
            OtherInitialFee = new FeeItem { Type = "fixed", Value = 25 },
            AnnualFee = new FeeItem { Type = "fixed", Value = 120 },
            OtherAnnualFee = new FeeItem { Type = "percent", Value = 0.5 },
            MonthlyManagementFee = new FeeItem { Type = "fixed", Value = 10 },
            OtherMonthlyFee = new FeeItem { Type = "percent", Value = 0.2 }
        };

        double effectiveRate = model.InterestRate / 100.0 / 12.0;
        double promoRate = model.PromoRate / 100.0 / 12.0;
        int mainPeriod = model.Months - model.PromoMonths - model.GraceMonths;

        double allFees = model.Amount * 0.015
                        + 50
                        + 25
                        + (120 + model.Amount * 0.005) * (model.Months / 12.0)
                        + (10 + model.Amount * 0.002) * model.Months;

        double promoPayment = model.Amount * promoRate / (1 - Math.Pow(1 + promoRate, -model.PromoMonths));
        double normalPayment = model.Amount * effectiveRate / (1 - Math.Pow(1 + effectiveRate, -mainPeriod));
        double expectedTotalPaid = promoPayment * model.PromoMonths + normalPayment * mainPeriod + allFees;

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(normalPayment, model.MonthlyPayment, 2);
        Assert.Equal(expectedTotalPaid, model.TotalPaid, 2);
        Assert.Equal(expectedTotalPaid - model.Amount, model.TotalInterest, 2);
    }

    [Fact]
    public void OnPost_Linear_ComputesTotals()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 6000,
            Months = 6,
            InterestRate = 6,
            PaymentType = "linear"
        };

        double effectiveRate = model.InterestRate / 100.0 / 12.0;
        double expectedTotal = 0;
        double remaining = model.Amount;
        for (int i = 0; i < model.Months; i++)
        {
            double principal = model.Amount / model.Months;
            double interest = remaining * effectiveRate;
            expectedTotal += principal + interest;
            remaining -= principal;
        }
        double expectedMonthlyPayment = model.Amount / model.Months + (model.Amount * effectiveRate);

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedMonthlyPayment, model.MonthlyPayment, 2);
        Assert.Equal(expectedTotal, model.TotalPaid, 2);
        Assert.Equal(expectedTotal - model.Amount, model.TotalInterest, 2);
    }

    [Fact]
    public void OnPost_ZeroMonths_Annuity_SetsZeroPayments()
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
    public void OnPost_ZeroMonths_Linear_ProducesInfinityMonthlyPayment()
    {
        // Arrange
        var model = new CreditModel
        {
            Amount = 3000,
            Months = 0,
            InterestRate = 8,
            PaymentType = "linear"
        };

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.True(double.IsPositiveInfinity(model.MonthlyPayment));
        Assert.Equal(0, model.TotalPaid, 2);
        Assert.Equal(-model.Amount, model.TotalInterest, 2);
    }
}
