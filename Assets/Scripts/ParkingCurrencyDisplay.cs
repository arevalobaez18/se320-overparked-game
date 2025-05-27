using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/*
public class ParkingCurrencyDisplay : MonoBehaviour
{
    public TextMeshProUGUI currencyText;
    private CurrencyContext currencyContext;

    private float simulatedRate = 4.25f;

    private void Start()
    {
        currencyContext = new CurrencyContext(new CorgiCoinFormatter());
        UpdateRateDisplay(simulatedRate);
    }

    public void SwitchToCorgiCoin()
    {
        currencyContext.SetStrategy(new CorgiCoinFormatter());
        UpdateRateDisplay(simulatedRate);
    }

    public void SwitchToChapmanTuition()
    {
        currencyContext.SetStrategy(new ChapmanTuitionFormatter());
        UpdateRateDisplay(simulatedRate);
    }

    public void UpdateRateDisplay(float rate)
    {
        currencyText.text = currencyContext.FormatAmount(rate) + " /hr";
    }
}
*/