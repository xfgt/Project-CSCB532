using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreditModel : PageModel
{
    [BindProperty] public double Amount { get; set; }
    [BindProperty] public int Months { get; set; }
    [BindProperty] public double InterestRate { get; set; }
    [BindProperty] public string PaymentType { get; set; } = "annuity";
//TODO ADD MISSING THINGS
    [BindProperty] public int PromoMonths { get; set; }
    [BindProperty] public double PromoRate { get; set; }
    [BindProperty] public int GraceMonths { get; set; }
    [BindProperty] public double Fees { get; set; }

    public bool ShowResults { get; set; } = false;
    public double MonthlyPayment { get; set; }
    public double TotalPaid { get; set; }
    public double TotalInterest { get; set; }

    public void OnPost()
    {
        double effectiveRate = InterestRate / 100 / 12;
        double promoRateMonthly = PromoRate / 100 / 12;

        int mainPeriod = Months - PromoMonths - GraceMonths;
        if (mainPeriod < 0) mainPeriod = 0;

        double promoPayment = 0;
        double normalPayment = 0;

        // Анюитетни вноски
        if (PaymentType == "annuity")
        {
            if (PromoMonths > 0 && PromoRate > 0)
                promoPayment = Amount * promoRateMonthly / (1 - Math.Pow(1 + promoRateMonthly, -PromoMonths));

            if (mainPeriod > 0)
                normalPayment = Amount * effectiveRate / (1 - Math.Pow(1 + effectiveRate, -mainPeriod));

            MonthlyPayment = normalPayment;
            TotalPaid = promoPayment * PromoMonths + normalPayment * mainPeriod + Fees;
        }
        else // Намаляващи вноски
        {
            double remaining = Amount;
            double total = 0;

            for (int i = 0; i < Months; i++)
            {
                double monthlyRate = (i < PromoMonths) ? promoRateMonthly : effectiveRate;
                double principal = Amount / Months;
                double interest = remaining * monthlyRate;
                total += principal + interest;
                remaining -= principal;
            }

            MonthlyPayment = Amount / Months + (Amount * effectiveRate);
            TotalPaid = total + Fees;
        }

        TotalInterest = TotalPaid - Amount;
        ShowResults = true;
    }
}
