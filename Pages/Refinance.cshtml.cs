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
    [BindProperty] public double NewFeePercent { get; set; }//proveri za kakwo sa
    [BindProperty] public double NewFeeFixed { get; set; }//TODO

    public bool ShowResults { get; set; }
    public double RemainingPrincipal { get; set; }
    public double EarlyFeeValue { get; set; }
    public double OldPayment { get; set; }
    public double NewPayment { get; set; }
    public double OldTotalCost { get; set; }
    public double NewFeeTotal { get; set; }
    public double NewTotalCost { get; set; }
    public double Saving { get; set; }

    /*public void OnPost()
    {
        //12 months has in one year
        double r1 = CurrentRate / 12;
        //double r= 0.03/12;
        double r2 = NewRate / 12;

        // Annuity formula
        double P1 =CurrentAmount*(r1/(1- Math.Pow((1+r1),-CurrentMonths)));

        
        // money you need to pay
        RemainingPrincipal = Math.Round(CurrentAmount - (P1*PaymentsMade));
  
        // Такса за предсрочно погасяване
        EarlyFeeValue = RemainingPrincipal * EarlyFee / 100;
        OldTotalCost = RemainingPrincipal + EarlyFeeValue;

        // Нов кредит
        NewPayment = RemainingPrincipal * newMonthlyRate / (1 - Math.Pow(1 + newMonthlyRate, -CurrentMonths));
        NewFeeTotal = (RemainingPrincipal * NewFeePercent / 100) + NewFeeFixed;
        NewTotalCost = NewPayment * CurrentMonths + NewFeeTotal;

        Saving = OldTotalCost - NewTotalCost;
        ShowResults = true;
    }
}*/
public void OnPost()
{
    double oldMonthlyRate = CurrentRate / 100 / 12;
    double newMonthlyRate = NewRate / 100 / 12;

    //Анюитетна вноска за стария кредит
    OldPayment = CurrentAmount * oldMonthlyRate / (1 - Math.Pow(1 + oldMonthlyRate, -CurrentMonths));

    //Оставаща главница след направени вноски
    RemainingPrincipal = CurrentAmount * Math.Pow(1 + oldMonthlyRate, PaymentsMade)
        - OldPayment * (Math.Pow(1 + oldMonthlyRate, PaymentsMade) - 1) / oldMonthlyRate;
    //RemainingPrincipal = 212 965,48;
    //Разликата трябва да е 53241,6
    Console.WriteLine(OldPayment);
    Console.WriteLine(CurrentAmount);
    Console.WriteLine(CurrentAmount * Math.Pow(1 + oldMonthlyRate, PaymentsMade));
    Console.WriteLine(OldPayment * (Math.Pow(1 + oldMonthlyRate, PaymentsMade) - 1) / oldMonthlyRate);
    Console.WriteLine(RemainingPrincipal);
    // Такса за предсрочно погасяване
    EarlyFeeValue = RemainingPrincipal * EarlyFee / 100;

    // Общо дължимо по стария кредит оттук нататък
    OldTotalCost = RemainingPrincipal + EarlyFeeValue;

    //Нов срок (ако рефинансираш оставащите месеци)
    int newMonths = CurrentMonths - PaymentsMade;

    // Анюитетна вноска за новия кредит
    NewPayment = RemainingPrincipal * newMonthlyRate / (1 - Math.Pow(1 + newMonthlyRate, -newMonths));

    //Такси по новия кредит
    NewFeeTotal = (RemainingPrincipal * NewFeePercent / 100) + NewFeeFixed;

    // Общо дължимо по новия кредит (всички вноски + такси)
    NewTotalCost = NewPayment * newMonths + NewFeeTotal;

    //Разлика (спестяване)
    Saving = OldTotalCost - NewTotalCost;

    ShowResults = true;
}
}
