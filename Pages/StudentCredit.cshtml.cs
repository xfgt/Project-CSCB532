using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

public class StudentCreditModel : PageModel
{
    // params
    // Amount of credit
    [BindProperty] public double Amount { get; set; }
    // Interest rate
    [BindProperty] public double Rate { get; set; }
    // Months to pay
    [BindProperty] public int Months { get; set; }
    // Months of grace
    [BindProperty] public int GraceMonths { get; set; }

    // results
    public bool ShowResults { get; set; }
    //monthly payment
    public double MonthlyPayment { get; set; }
    //total paid
    public double TotalPaid { get; set; }
    //total interest
    public double TotalInterest { get; set; }

    public void OnPost()
    {
        //calculate
        // monthly rate
        double monthlyRate = Rate / 100 / 12;
        // repay months after removing grace months
        int repayMonths = Months - GraceMonths;
        // monthly payment
        MonthlyPayment = Amount * monthlyRate /
            (1 - Math.Pow(1 + monthlyRate, -repayMonths));
        // total paid
        TotalPaid = MonthlyPayment * repayMonths;
        // total interest
        TotalInterest = TotalPaid - Amount;
        // show results
        ShowResults = true;
    }
}
