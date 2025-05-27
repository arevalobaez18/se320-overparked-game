public class DollarSignCurrencyDisplayStrategy : ICurrencyDisplayStrategy
{
    public string Display(int currencyAmount)
    {
        return "$" + currencyAmount;
    }
}
