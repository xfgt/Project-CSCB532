using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

public class RefinanceModel : PageModel
{
    [BindProperty] public double CurrentAmount { get; set; }
    [BindProperty] public double CurrentRate { get; set; }
    [BindProperty] public int CurrentMonths { get; set; }
    [BindProperty] public int PaymentsMade { get; set; }
    [BindProperty] public double EarlyFee { get; set; }

    [BindProperty] public double NewRate { get; set; }
    [BindProperty] public double NewFeePercent { get; set; }
    [BindProperty] public double NewFeeFixed { get; set; }

    public bool ShowResults { get; set; } = false;
    public double RemainingPrincipal { get; set; }
    public double OldTotalCost { get; set; }
    public double NewTotalCost { get; set; }
    public double Saving { get; set; }

    public void OnPost()
    {
        double oldMonthlyRate = CurrentRate / 100 / 12;
        double newMonthlyRate = NewRate / 100 / 12;

        // Изчисляваме анюитетна вноска за стария кредит
        double oldPayment = CurrentAmount * oldMonthlyRate / (1 - Math.Pow(1 + oldMonthlyRate, -CurrentMonths));

        // Оставаща главница
        RemainingPrincipal = CurrentAmount * Math.Pow(1 + oldMonthlyRate, PaymentsMade)
            - oldPayment * (Math.Pow(1 + oldMonthlyRate, PaymentsMade) - 1) / oldMonthlyRate;

        double earlyPayFee = RemainingPrincipal * EarlyFee / 100;
        OldTotalCost = RemainingPrincipal + earlyPayFee;

        // Нов кредит
        double newPayment = RemainingPrincipal * newMonthlyRate / (1 - Math.Pow(1 + newMonthlyRate, -CurrentMonths));
        NewTotalCost = newPayment * CurrentMonths + (RemainingPrincipal * NewFeePercent / 100) + NewFeeFixed;

        Saving = OldTotalCost - NewTotalCost;
        ShowResults = true;
    }
}
