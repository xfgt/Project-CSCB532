using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LeasingModel : PageModel
{
    [BindProperty] public double Price { get; set; }
    [BindProperty] public int Months { get; set; }
    [BindProperty] public double FeePercent { get; set; }
    [BindProperty] public double DownPayment { get; set; }
    [BindProperty] public double MonthlyPaymentInput { get; set; }

    public bool ShowResults { get; set; } = false;
    public double APR { get; set; }
    public double TotalPaid { get; set; }
    public double TotalFees { get; set; }

    public void OnPost()
    {
        
        double loanAmount = Price - DownPayment; // amount actually borrowed
        double processingFee = Price * (FeePercent / 100);
        TotalFees = processingFee;

        TotalPaid = MonthlyPaymentInput * Months + DownPayment + processingFee;

        double totalInterestAndFees = TotalPaid - loanAmount;
        double years = Months / 12.0;

        // APR formula
        APR = (totalInterestAndFees / loanAmount) / years * 100;

        ShowResults = true;

    }
}
