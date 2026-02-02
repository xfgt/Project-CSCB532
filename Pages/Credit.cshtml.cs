using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class FeeItem
{
    public double Value { get; set; }
    public string Type { get; set; } = "fixed"; // "percent" or "fixed"
}


public class CreditModel : PageModel
{
    /// Credit calculation model
    
    // Amount you need to get
    [BindProperty] public double Amount { get; set; }
    // How much months you will pay?
    [BindProperty] public int Months { get; set; }
    // The creditors will give you rate
    [BindProperty] public double InterestRate { get; set; }
    // Types : Annuity or Linear and also put annuity as base
    [BindProperty] public string PaymentType { get; set; } = "annuity";
    // Promotion months put
    [BindProperty] public int PromoMonths { get; set; }
    //lower rate or just free month without pay
    [BindProperty] public double PromoRate { get; set; }
    // grace months
    [BindProperty] public int GraceMonths { get; set; }

    // Fees for candidate
    [BindProperty] public FeeItem ApplicationFee { get; set; } = new();
    // Processing fee(obrabotka)
    [BindProperty] public FeeItem ProcessingFee { get; set; } = new();
    // Initial fee purvonachalni taxs
    [BindProperty] public FeeItem OtherInitialFee { get; set; } = new();
    //  Yearly fee
    [BindProperty] public FeeItem AnnualFee { get; set; } = new();
    // Yearly others fees
    [BindProperty] public FeeItem OtherAnnualFee { get; set; } = new();
    // Monthly management fee
    [BindProperty] public FeeItem MonthlyManagementFee { get; set; } = new();
    // Monthly others fees
    [BindProperty] public FeeItem OtherMonthlyFee { get; set; } = new();

    // Results
    public bool ShowResults { get; set; }
    // Monthly payment to pay
    public double MonthlyPayment { get; set; }
    // total receipt to pay to final
    public double TotalPaid { get; set; }
    // how you paid upper the amount
    public double TotalInterest { get; set; }

    // public void OnPost()
    // {
    //     // params
    //     // loanAmount is the money needed to pay off the loan
    //     // processingFee is the processing fee
    //     // TotalFees is all the fees paid for the loan
    //     // TotalPaid is all the money paid for the loan

    //     // Effective rate for 1 month
    //     double effectiveRate = InterestRate / 100.0 / 12.0;
    //     // Promo rate for 1 month
    //     double promoRateMonthly = PromoRate / 100.0 / 12.0;
    //     // All months without promotions
    //     int mainPeriod = Months - PromoMonths - GraceMonths;
    //     // then bankrupt the bank
    //     if (mainPeriod < 0) mainPeriod = 0;

    //     //counting all fees
    //     double allFees = CalculateFee(ApplicationFee)
    //                     + CalculateFee(ProcessingFee)
    //                     + CalculateFee(OtherInitialFee)
    //                     + (CalculateFee(AnnualFee) + CalculateFee(OtherAnnualFee)) * (Months / 12.0)
    //                     + (CalculateFee(MonthlyManagementFee) + CalculateFee(OtherMonthlyFee)) * Months;
    //     // promoPayment and normalpayment from zero, just to avoid errors
    //     double promoPayment = 0;
    //     double normalPayment = 0;

    //     //base annuity if it is
    //     if (PaymentType == "annuity")
    //     {
    //         //if there is a promotion and also rate
    //         if (PromoMonths > 0 && PromoRate > 0)
    //             promoPayment = Amount * promoRateMonthly / (1 - Math.Pow(1 + promoRateMonthly, -PromoMonths));
    //         // if there is no promotion
    //         if (mainPeriod > 0)
    //             normalPayment = Amount * effectiveRate / (1 - Math.Pow(1 + effectiveRate, -mainPeriod));
    //         //if there is nor promotion, then it is monthly payment
    //         MonthlyPayment = normalPayment;
    //         //total paid
    //         TotalPaid = promoPayment * PromoMonths + normalPayment * mainPeriod + allFees;
    //     }
    //     //if it is linear loan
    //     else
    //     {
    //         // how much is left
    //         double remaining = Amount;
    //         // starting from zero to avoid errors
    //         double total = 0;
    //         //for each month
    //         for (int i = 0; i < Months; i++)
    //         {
    //             // if there is a promotion
    //             double rate = (i < PromoMonths) ? promoRateMonthly : effectiveRate;
    //             // calculate the monthly payment without interest/fees
    //             double principal = Amount / Months;
    //             // calculate the interest
    //             double interest = remaining * rate;
    //             // add to total
    //             total += principal + interest;
    //             // subtract from remaining or removing partial of remaining
    //             remaining -= principal;
    //         }
    //         // add fees and total
    //         MonthlyPayment = Amount / Months + (Amount * effectiveRate);
    //         TotalPaid = total + allFees;
    //     }
    //     //total interest
    //     TotalInterest = TotalPaid - Amount;
    //     //show results
    //     ShowResults = true;
    // }
    
    public void OnPost(){
        
        var result = CreditCalculator.Calculate(
            Amount,
            Months,
            InterestRate,
            PaymentType,
            PromoMonths,
            PromoRate,
            GraceMonths,
            ApplicationFee,
            ProcessingFee,
            OtherInitialFee,
            AnnualFee,
            OtherAnnualFee,
            MonthlyManagementFee,
            OtherMonthlyFee
        );

        MonthlyPayment = result.monthly;
        TotalPaid = result.totalPaid;
        TotalInterest = result.totalInterest;
        //show results
        ShowResults = true;        

    }

    // // calculate fee
    // private double CalculateFee(FeeItem fee)
    // {
    //     return fee.Type == "percent" ? Amount * (fee.Value / 100.0) : fee.Value;
    // }
}
