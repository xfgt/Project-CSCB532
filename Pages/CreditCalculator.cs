public class CreditCalculator
{
    public static (double monthly, double totalPaid, double totalInterest) Calculate(
        double amount,
        int months,
        double interestRate,
        string paymentType,
        int promoMonths,
        double promoRate,
        int graceMonths,
        FeeItem applicationFee,
        FeeItem processingFee,
        FeeItem otherInitialFee,
        FeeItem annualFee,
        FeeItem otherAnnualFee,
        FeeItem monthlyFee,
        FeeItem otherMonthlyFee)
    {
        // params
        // loanAmount is the money needed to pay off the loan
        // processingFee is the processing fee
        // TotalFees is all the fees paid for the loan
        // TotalPaid is all the money paid for the loan

        // Effective rate for 1 month
        double effectiveRate = interestRate / 100.0 / 12.0;
        // Promo rate for 1 month
        double promoRateMonthly = promoRate / 100.0 / 12.0;
        int mainPeriod = months;
        // All months without promotions
        if (promoMonths > 0 && promoRate > 0){
            mainPeriod -=  (promoMonths + graceMonths);
        } else {
            mainPeriod -= graceMonths;
        }
        // then bankrupt the bank
        if (mainPeriod < 0) mainPeriod = 0;

        double graceTotal = 0;

        for (int i = 0; i < graceMonths; i++)
        {
            // if there is a grace
            graceTotal += amount * effectiveRate;
        }
         

        //counting all fees
        double allFees =
            CalculateFee(amount, applicationFee) +
            CalculateFee(amount, processingFee) +
            CalculateFee(amount, otherInitialFee) +
            (CalculateFee(amount, annualFee) + CalculateFee(amount, otherAnnualFee)) * (months / 12.0) +
            (CalculateFee(amount, monthlyFee) + CalculateFee(amount, otherMonthlyFee)) * months;

        // monthly payment
        double monthlyPayment;
        // total paid
        double totalPaid;

        //base annuity if it is
        if (paymentType == "annuity")
        {
            // promoPayment and normalpayment from zero, just to avoid errors
            double promoPayment = 0;
            double normalPayment = 0;

            //if there is a promotion and also rate
            if (promoMonths > 0 && promoRate > 0)
                promoPayment = amount * promoRateMonthly / (1 - Math.Pow(1 + promoRateMonthly, -promoMonths));
            // if there is no promotion
            if (mainPeriod > 0)
                normalPayment = amount * effectiveRate / (1 - Math.Pow(1 + effectiveRate, -mainPeriod));
            //if there is nor promotion, then it is monthly payment
            monthlyPayment = normalPayment;
            //total paid
            totalPaid = promoPayment * promoMonths + normalPayment * mainPeriod + allFees;
        }
        //if it is linear loan
        else
        {
            // how much is left
            double remaining = amount;
            // starting from zero to avoid errors
            double total = 0;
            //for each month
            for (int i = 0; i < months; i++)
            {
                // if there is a promotion
                double rate = (i < promoMonths) ? promoRateMonthly : effectiveRate;
                // calculate the monthly payment without interest/fees
                double principal = amount / months;
                // calculate the interest
                double interest = remaining * rate;
                // add to total
                total += principal + interest;
                // subtract from remaining or removing partial of remaining
                remaining -= principal;
            }
            // add fees and total
            monthlyPayment = amount / months + (amount * effectiveRate);
            totalPaid = total + allFees;
        }
        
        totalPaid += graceTotal;

        //total interest
        //TotalInterest = TotalPaid - Amount; 
        return (monthlyPayment, totalPaid, totalPaid - amount);
        
    }

    // calculate fee
    private static double CalculateFee(double amount, FeeItem fee)
    {
        return fee.Type == "percent"
            ? amount * (fee.Value / 100.0)
            : fee.Value;
    }
}