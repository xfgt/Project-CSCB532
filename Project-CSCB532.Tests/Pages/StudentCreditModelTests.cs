using System;
using Xunit;

namespace Project_CSCB532.Tests.Pages;

public class StudentCreditModelTests
{
    [Fact]
    public void OnPost_WithGracePeriod_ComputesExpectedValues()
    {
        // Arrange
        var model = new StudentCreditModel
        {
            Amount = 10000,
            Rate = 12,
            Months = 12,
            GraceMonths = 2
        };

        double monthlyRate = model.Rate / 100 / 12;
        int repayMonths = model.Months - model.GraceMonths;
        double expectedMonthlyPayment = model.Amount * monthlyRate / (1 - Math.Pow(1 + monthlyRate, -repayMonths));
        double expectedTotalPaid = expectedMonthlyPayment * repayMonths;
        double expectedTotalInterest = expectedTotalPaid - model.Amount;

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedMonthlyPayment, model.MonthlyPayment, 2);
        Assert.Equal(expectedTotalPaid, model.TotalPaid, 2);
        Assert.Equal(expectedTotalInterest, model.TotalInterest, 2);
    }

    [Fact]
    public void OnPost_ZeroRate_ProducesNaNPayments()
    {
        // Arrange
        var model = new StudentCreditModel
        {
            Amount = 5000,
            Rate = 0,
            Months = 10,
            GraceMonths = 0
        };

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.True(double.IsNaN(model.MonthlyPayment));
        Assert.True(double.IsNaN(model.TotalPaid));
        Assert.True(double.IsNaN(model.TotalInterest));
    }

    [Fact]
    public void OnPost_GraceEqualsMonths_ProducesInfiniteMonthlyPayment()
    {
        // Arrange
        var model = new StudentCreditModel
        {
            Amount = 8000,
            Rate = 5,
            Months = 12,
            GraceMonths = 12
        };

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.True(double.IsPositiveInfinity(model.MonthlyPayment));
        Assert.True(double.IsNaN(model.TotalPaid));
        Assert.True(double.IsNaN(model.TotalInterest));
    }
}
