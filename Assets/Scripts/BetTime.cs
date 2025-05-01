using System;
using UnityEngine;
using UnityEngine.UI;

public class BetTime : MonoBehaviour
{
    // Assign in inspector to display the text
    public Text legacyText;

    private DateTime betEndTime;

    void Start()
    {
        // Get current real-world time
        DateTime currentTime = DateTime.Now;

        // Set bet end time to 5 minutes from now
        betEndTime = currentTime.AddMinutes(5);
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SkipTime(30f);  // Skip 30 seconds on the timer after pressing F3 (Editor-Exclusive)
        }
        #endif
        
        TimeSpan timeLeft = betEndTime - DateTime.Now;

        if (timeLeft.TotalSeconds > 0)
        {
            string minutes = Mathf.FloorToInt((float)timeLeft.TotalMinutes).ToString();
            string seconds = Mathf.FloorToInt((float)(timeLeft.Seconds)).ToString("D2");

            // Display legacy style time text
            legacyText.text = $"Bets close in: {minutes}:{seconds} " +
                              $"\n(ends: {betEndTime:hh:mm tt})";
        }
        else
        {
            legacyText.text = "Betting window closed.";
        }
    }
    
    // Debug Functions
    private void SkipTime(float seconds)
    {
        betEndTime = betEndTime.AddSeconds(-seconds);
        Debug.Log($"[DEBUG] Skipped {seconds} seconds. New end time: {betEndTime:hh:mm:ss tt}");
    }
}