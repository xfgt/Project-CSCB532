using System;
using Xunit;

namespace Project_CSCB532.Tests.Pages;

public class RefinanceModelTests
{
    private static (double OldPayment, double RemainingPrincipal, double EarlyFeeValue, double OldTotalCost, double NewPayment, double NewFeeTotal, double NewTotalCost, double Saving) CalculateExpected(
        double currentAmount,
        double currentRate,
        int currentMonths,
        int paymentsMade,
        double earlyFee,
        double newRate,
        double newFeePercent,
        double newFeeFixed)
    {
        double oldMonthlyRate = currentRate / 100 / 12;
        double newMonthlyRate = newRate / 100 / 12;

        double oldPayment = currentAmount * oldMonthlyRate / (1 - Math.Pow(1 + oldMonthlyRate, -currentMonths));

        double remainingPrincipal = currentAmount * Math.Pow(1 + oldMonthlyRate, paymentsMade)
            - oldPayment * (Math.Pow(1 + oldMonthlyRate, paymentsMade) - 1) / oldMonthlyRate;

        double earlyFeeValue = remainingPrincipal * earlyFee / 100;
        double oldTotalCost = ((oldPayment * currentMonths) + earlyFeeValue) - (oldPayment * paymentsMade);

        int newMonths = currentMonths - paymentsMade;
        double newPayment = remainingPrincipal * newMonthlyRate / (1 - Math.Pow(1 + newMonthlyRate, -newMonths));
        double newFeeTotal = (remainingPrincipal * newFeePercent / 100) + newFeeFixed;
        double newTotalCost = newPayment * newMonths + newFeeTotal;

        double saving = oldTotalCost - newTotalCost;
        return (oldPayment, remainingPrincipal, earlyFeeValue, oldTotalCost, newPayment, newFeeTotal, newTotalCost, saving);
    }

    [Fact]
    public void OnPost_LowerNewRate_ProducesSavings()
    {
        // Arrange
        var model = new RefinanceModel
        {
            CurrentAmount = 200000,
            CurrentRate = 5,
            CurrentMonths = 240,
            PaymentsMade = 60,
            EarlyFee = 1,
            NewRate = 3,
            NewFeePercent = 1,
            NewFeeFixed = 500
        };

        var expected = CalculateExpected(model.CurrentAmount, model.CurrentRate, model.CurrentMonths, model.PaymentsMade,
            model.EarlyFee, model.NewRate, model.NewFeePercent, model.NewFeeFixed);

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expected.OldPayment, model.OldPayment, 2);
        Assert.Equal(expected.RemainingPrincipal, model.RemainingPrincipal, 2);
        Assert.Equal(expected.EarlyFeeValue, model.EarlyFeeValue, 2);
        Assert.Equal(expected.OldTotalCost, model.OldTotalCost, 2);
        Assert.Equal(expected.NewPayment, model.NewPayment, 2);
        Assert.Equal(expected.NewFeeTotal, model.NewFeeTotal, 2);
        Assert.Equal(expected.NewTotalCost, model.NewTotalCost, 2);
        Assert.Equal(expected.Saving, model.Saving, 2);
        Assert.True(model.Saving > 0);
    }

    [Fact]
    public void OnPost_HigherNewRate_ReducesSavings()
    {
        // Arrange
        var model = new RefinanceModel
        {
            CurrentAmount = 150000,
            CurrentRate = 3,
            CurrentMonths = 180,
            PaymentsMade = 24,
            EarlyFee = 0.5,
            NewRate = 4,
            NewFeePercent = 0.5,
            NewFeeFixed = 300
        };

        var expected = CalculateExpected(model.CurrentAmount, model.CurrentRate, model.CurrentMonths, model.PaymentsMade,
            model.EarlyFee, model.NewRate, model.NewFeePercent, model.NewFeeFixed);

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.Equal(expected.OldPayment, model.OldPayment, 2);
        Assert.Equal(expected.RemainingPrincipal, model.RemainingPrincipal, 2);
        Assert.Equal(expected.EarlyFeeValue, model.EarlyFeeValue, 2);
        Assert.Equal(expected.OldTotalCost, model.OldTotalCost, 2);
        Assert.Equal(expected.NewPayment, model.NewPayment, 2);
        Assert.Equal(expected.NewFeeTotal, model.NewFeeTotal, 2);
        Assert.Equal(expected.NewTotalCost, model.NewTotalCost, 2);
        Assert.Equal(expected.Saving, model.Saving, 2);
        Assert.True(model.Saving < 0 || Math.Abs(model.Saving) < 0.01);
    }

    [Fact]
    public void OnPost_PaymentsEqualTerm_ProducesInfiniteNewPayment()
    {
        // Arrange
        var model = new RefinanceModel
        {
            CurrentAmount = 100000,
            CurrentRate = 4,
            CurrentMonths = 12,
            PaymentsMade = 12,
            EarlyFee = 1,
            NewRate = 3,
            NewFeePercent = 1,
            NewFeeFixed = 200
        };

        // Act
        model.OnPost();

        // Assert
        Assert.True(model.ShowResults);
        Assert.True(double.IsInfinity(model.NewPayment));
        Assert.True(double.IsNaN(model.NewTotalCost));
        Assert.True(double.IsNaN(model.Saving));
    }
}
