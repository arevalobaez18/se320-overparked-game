public class BasicCurrencyDisplayStrategy : ICurrencyDisplayStrategy
{
    public string Display(int currencyAmount)
    {
        return "" + currencyAmount;
    }
}
