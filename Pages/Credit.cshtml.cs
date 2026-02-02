using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class FeeItem
{
    public double Value { get; set; }
    public string Type { get; set; } = "fixed"; // "percent" or "fixed"
}

public class CreditModel : PageModel
{
    [BindProperty] public double Amount { get; set; }
    [BindProperty] public int Months { get; set; }
    [BindProperty] public double InterestRate { get; set; }
    [BindProperty] public string PaymentType { get; set; } = "annuity";

    [BindProperty] public int PromoMonths { get; set; }
    [BindProperty] public double PromoRate { get; set; }
    [BindProperty] public int GraceMonths { get; set; }

    
    [BindProperty] public FeeItem ApplicationFee { get; set; } = new();
    [BindProperty] public FeeItem ProcessingFee { get; set; } = new();
    [BindProperty] public FeeItem OtherInitialFee { get; set; } = new();

    [BindProperty] public FeeItem AnnualFee { get; set; } = new();
    [BindProperty] public FeeItem OtherAnnualFee { get; set; } = new();

    [BindProperty] public FeeItem MonthlyManagementFee { get; set; } = new();
    [BindProperty] public FeeItem OtherMonthlyFee { get; set; } = new();

    public bool ShowResults { get; set; }
    public double MonthlyPayment { get; set; }
    public double TotalPaid { get; set; }
    public double TotalInterest { get; set; }

    public void OnPost()
    {
        double effectiveRate = InterestRate / 100.0 / 12.0;
        double promoRateMonthly = PromoRate / 100.0 / 12.0;
        int mainPeriod = Months - PromoMonths - GraceMonths;
        if (mainPeriod < 0) mainPeriod = 0;

        
        double allFees = CalculateFee(ApplicationFee)
                        + CalculateFee(ProcessingFee)
                        + CalculateFee(OtherInitialFee)
                        + (CalculateFee(AnnualFee) + CalculateFee(OtherAnnualFee)) * (Months / 12.0)
                        + (CalculateFee(MonthlyManagementFee) + CalculateFee(OtherMonthlyFee)) * Months;

        double promoPayment = 0;
        double normalPayment = 0;

        

        if (PaymentType == "annuity")
        {
            if (PromoMonths > 0 && PromoRate > 0)
                promoPayment = Amount * promoRateMonthly / (1 - Math.Pow(1 + promoRateMonthly, -PromoMonths));

            if (mainPeriod > 0)
                normalPayment = Amount * effectiveRate / (1 - Math.Pow(1 + effectiveRate, -mainPeriod));

            MonthlyPayment = normalPayment;
            TotalPaid = promoPayment * PromoMonths + normalPayment * mainPeriod + allFees;
        }
        else
        {
            double remaining = Amount;
            double total = 0;
            for (int i = 0; i < Months; i++)
            {
                double rate = (i < PromoMonths) ? promoRateMonthly : effectiveRate;
                double principal = Amount / Months;
                double interest = remaining * rate;
                total += principal + interest;
                remaining -= principal;
            }

            MonthlyPayment = Amount / Months + (Amount * effectiveRate);
            TotalPaid = total + allFees;
        }

        TotalInterest = TotalPaid - Amount;
        ShowResults = true;
    }

    private double CalculateFee(FeeItem fee)
    {
        return fee.Type == "percent" ? Amount * (fee.Value / 100.0) : fee.Value;
    }
}
