using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorgiCoinFormatter : CurrencySwap
{
    public string currencySwapFormatter(float amount)
    {
        return $"${amount:F2} Corgi Coin";
    }
}
