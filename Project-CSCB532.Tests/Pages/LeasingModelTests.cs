using System;
using Xunit;

namespace Project_CSCB532.Tests.Pages;

public class LeasingModelTests
{
    private static double CalculateExpectedApr(double price, double downPayment, int months, double monthlyPaymentInput)
    {
        double loanAmount = price - downPayment;
        if (months == 0)
        {
            return 0;
        }

        double low = 0.0;
        double high = 1.0;
        double mid = 0.0;

        for (int i = 0; i < 100; i++)
        {
            mid = (low + high) / 2.0;
            double guessPayment = (loanAmount * mid) / (1 - Math.Pow(1 + mid, -months));

            if (double.IsInfinity(guessPayment) || guessPayment > monthlyPaymentInput)
                high = mid;
            else
                low = mid;
        }

        double monthlyRate = mid;
        return (Math.Pow(1 + monthlyRate, 12) - 1) * 100;
    }

    [Fact]
    public void OnPost_PercentFee_ComputesTotalsAndApr()
    {
        // Arrange
        var model = new LeasingModel
        {
            Price = 10000,
            DownPayment = 2000,
            Months = 12,
            FeeType = "percent",
            FeeValue = 2.0
        };

        double loanAmount = model.Price - model.DownPayment;
        double targetMonthlyRate = 0.01; // 1% monthly
        double monthlyPayment = (loanAmount * targetMonthlyRate) / (1 - Math.Pow(1 + targetMonthlyRate, -model.Months));
        model.MonthlyPaymentInput = monthlyPayment;

        double processingFee = model.Price * (model.FeeValue / 100.0);
        double expectedTotalFees = processingFee;
        double expectedTotalPaid = monthlyPayment * model.Months + model.DownPayment + processingFee;
        double expectedApr = CalculateExpectedApr(model.Price, model.DownPayment, model.Months, monthlyPayment);

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedTotalFees, model.TotalFees, 2);
        Assert.Equal(expectedTotalPaid, model.TotalPaid, 2);
        Assert.Equal(expectedApr, model.APR, 2);
    }

    [Fact]
    public void OnPost_FixedFee_ComputesTotalsAndApr()
    {
        // Arrange
        var model = new LeasingModel
        {
            Price = 5000,
            DownPayment = 500,
            Months = 10,
            FeeType = "fixed",
            FeeValue = 100
        };

        double loanAmount = model.Price - model.DownPayment;
        double targetMonthlyRate = 0.015; // 1.5% monthly
        double monthlyPayment = (loanAmount * targetMonthlyRate) / (1 - Math.Pow(1 + targetMonthlyRate, -model.Months));
        model.MonthlyPaymentInput = monthlyPayment;

        double processingFee = model.FeeValue;
        double expectedTotalFees = processingFee;
        double expectedTotalPaid = monthlyPayment * model.Months + model.DownPayment + processingFee;
        double expectedApr = CalculateExpectedApr(model.Price, model.DownPayment, model.Months, monthlyPayment);

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedTotalFees, model.TotalFees, 2);
        Assert.Equal(expectedTotalPaid, model.TotalPaid, 2);
        Assert.Equal(expectedApr, model.APR, 2);
    }

    [Fact]
    public void OnPost_ZeroMonths_SetsAprToZero()
    {
        // Arrange
        var model = new LeasingModel
        {
            Price = 3000,
            DownPayment = 1000,
            Months = 0,
            FeeType = "percent",
            FeeValue = 5,
            MonthlyPaymentInput = 200
        };

        double processingFee = model.Price * (model.FeeValue / 100.0);
        double expectedTotalFees = processingFee;
        double expectedTotalPaid = model.MonthlyPaymentInput * model.Months + model.DownPayment + processingFee;
        double expectedApr = 0;

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expectedTotalFees, model.TotalFees, 2);
        Assert.Equal(expectedTotalPaid, model.TotalPaid, 2);
        Assert.Equal(expectedApr, model.APR, 2);
    }
}
