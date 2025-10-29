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
        
        double processingFee = Price * (FeePercent / 100);
        TotalPaid = MonthlyPaymentInput * Months + DownPayment + processingFee;
        //до тук вярно
        TotalFees = Price*(FeePercent/100);
        
        n = Months;
        double loanAmount = Price - DownPayment;


        // Годишен процентен разход (приблизително)
        
        //APR = (TotalFees / loanAmount) / (Months / 12.0) * 100;
        //double processingFee = Price * (FeePercent / 100);
        //TotalPaid = MonthlyPaymentInput * Months + DownPayment + processingFee;
        //TotalFees = Price * (FeePercent / 100);

        //double loanAmount = Price - DownPayment;

        // месечна лихва приблизително
        //double monthlyRate = ((MonthlyPaymentInput * Months + processingFee) - loanAmount) / (loanAmount * Months);

        // изчисляваме (1 + monthlyRate)^12 без pow()
        //double onePlus = 1.0 + monthlyRate;
        //double yearFactor = 1.0;
        //for (int i = 0; i < 12; i++) yearFactor *= onePlus;

        //APR = (yearFactor - 1.0) * 100;

        //ShowResults = true;
        //double loanAmount = Price - DownPayment;
        //double payment = MonthlyPaymentInput;
        int n = Months;

        // намираме месечната лихва чрез бинарно търсене
double low = 0.0, high = 1.0, r = 0.0;
for (int iter = 0; iter < 100; iter++) {
    r = (low + high) / 2.0;
    double presentValue = 0.0;
    double factor = 1.0;
    for (int i = 0; i < n; i++) {
        factor *= (1.0 + r);
        presentValue += payment / factor;
    }
    if (presentValue > loanAmount)
        low = r;     // лихвата е по-голяма
    else
        high = r;
}

        // преобразуваме в годишен процент (без pow)
        double yearFactor = 1.0;
        for (int i = 0; i < 12; i++) yearFactor *= (1.0 + r);
//APR = (yearFactor - 1.0) * 100; 
        ShowResults = true;

    }
}
