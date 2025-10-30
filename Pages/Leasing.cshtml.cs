using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LeasingModel : PageModel
{
    /// Leasing calculation model
    
    /// Price of the item to lease
    [BindProperty] public double Price { get; set; }
    /// Number of months to lease
    [BindProperty] public int Months { get; set; }
    /// Processing fee percentage
    [BindProperty] public double FeePercent { get; set; }
    [BindProperty] public string FeeType { get; set; } = "0"; // "percent" или "fixed"
[BindProperty] public double FeeValue { get; set; } // Въведената стойност
    /// Down payment amount
    [BindProperty] public double DownPayment { get; set; }
    /// Monthly payment
    [BindProperty] public double MonthlyPaymentInput { get; set; }

    

    //Results
    public bool ShowResults { get; set; } = false;
    //APR - Annual Percentage Rate by year
    public double APR { get; set; }
    //Total paid the loan
    public double TotalPaid { get; set; }
    //Total fees
    public double TotalFees { get; set; }

    public void OnPost()
    {
        /*params
        * loanAmount is the money needed to pay off the loan
        * processingFee is the processing fee
        * TotalFees is all the fees paid for the loan
        * TotalPaid is all the money paid for the loan
        */
        double loanAmount = Price - DownPayment;
        //double processingFee = Price * (FeePercent / 100);
        double processingFee;

if (FeeType == "fixed")
{
    processingFee = FeeValue; // фиксирана сума в лева
}
else
{
    processingFee = Price * (FeeValue / 100.0); // процент
}


        
        TotalFees = processingFee;

        TotalPaid = MonthlyPaymentInput * Months + DownPayment + processingFee;

        double totalInterestAndFees = TotalPaid - loanAmount;
        double years = Months / 12.0;

        // APR formula
        // TODO: Need asking for APR how to work properly
        APR = (totalInterestAndFees / loanAmount) / years * 100;
        
        ShowResults = true;

    }
}
