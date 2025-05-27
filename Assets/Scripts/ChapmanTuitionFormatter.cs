using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapmanTuitionFormatter : CurrencySwap
{
    private float conversionRate = 63000f;

    public string currencySwapFormatter(float amount)
    {
        float converted = amount * conversionRate;
        return $"${converted:F2} USD";
    }
}

