using UnityEngine;
using UnityEngine.UI; // For legacy Text

public class LoggedBet : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Text betText; // Assign in prefab

    // Sets the bet log text.
    public void SetBetInfo(int amount, bool isHigher, string structureName)
    {
        string direction = isHigher ? "higher" : "lower";
        betText.text = $"Placed {amount} coin on {direction} for {structureName}.";
    }
}