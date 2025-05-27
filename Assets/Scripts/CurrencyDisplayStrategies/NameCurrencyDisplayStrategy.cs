public class NameCurrencyDisplayStrategy : ICurrencyDisplayStrategy
{
    public string Display(int currencyAmount)
    {
        return currencyAmount + " SANDIE";
    }
}
