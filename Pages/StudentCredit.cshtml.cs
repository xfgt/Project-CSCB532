using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

public class StudentCreditModel : PageModel
{
    [BindProperty] public double Amount { get; set; }
    [BindProperty] public double Rate { get; set; }
    [BindProperty] public int Months { get; set; }
    [BindProperty] public int GraceMonths { get; set; }

    public bool ShowResults { get; set; }
    public double MonthlyPayment { get; set; }
    public double TotalPaid { get; set; }
    public double TotalInterest { get; set; }

    public void OnPost()
    {
        double monthlyRate = Rate / 100 / 12;
        int repayMonths = Months - GraceMonths;

        MonthlyPayment = Amount * monthlyRate /
            (1 - Math.Pow(1 + monthlyRate, -repayMonths));

        TotalPaid = MonthlyPayment * repayMonths;
        TotalInterest = TotalPaid - Amount;

        ShowResults = true;
    }
}
