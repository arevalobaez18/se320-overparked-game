using System;
using UnityEngine;
using UnityEngine.UI;

public class BetTime : MonoBehaviour
{
    // Assign in inspector to display the text
    public Text timeText;

    private DateTime betEndTime;

    // Track bet info
    private int betAmount = 0;
    private string betStructureName = "";

    private bool isActive = true; // Track if the bet window is active
    private bool rewardGiven = false; // Prevent multiple rewards

    public AudioClip timeUpSound; // Assign in inspector
    private AudioSource audioSource; // Internal reference

    void Start()
    {
        // Get current real-world time
        DateTime currentTime = DateTime.Now;

        // Set bet end time to 5 minutes from now
        betEndTime = currentTime.AddMinutes(5);

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SkipTime(30f);  // Skip 30 seconds on the timer after pressing F3 (Editor-Exclusive)
        }

        TimeSpan timeLeft = betEndTime - DateTime.Now;

        if (timeLeft.TotalSeconds > 0)
        {
            UpdateBetTime(timeLeft); // Update the time text
        }
        else
        {
            if (isActive)
            {
                EndBet(); // Call to end the bet
            }
        }
    }

    // Update the bet time text
    private void UpdateBetTime(TimeSpan timeLeft)
    {
        string minutes = Mathf.FloorToInt((float)timeLeft.TotalMinutes).ToString();
        string seconds = Mathf.FloorToInt((float)(timeLeft.Seconds)).ToString("D2");

      
        timeText.text = $"Bets close in: {minutes}:{seconds} " +
                          $"\n(ends: {betEndTime:hh:mm tt})";
    }

    public void StartBet(int amount, string structureName)
    {
        betAmount = amount;
        betStructureName = structureName;
        rewardGiven = false;
        Debug.Log($"[BetTime] Bet started: {amount} on {structureName}");
    }


    public void EndBet()
    {
        isActive = false; 
        Debug.Log($"[BetTime] Betting window closed for {betStructureName}.");

     
        int initialCurrency = 100;

        
        int currencyDifference = CurrencyManager.Instance.currency - initialCurrency;

        
        string resultMessage = "You lost the bet!";  

        if (currencyDifference > 0)
        {
            
            resultMessage = $"You won {currencyDifference} currency!";
        }
        else if (currencyDifference < 0)
        {
            
            resultMessage = $"You lost {-currencyDifference} currency!";
        }
        else
        {
            
            resultMessage = "You broke even. No gain, no loss!";
        }

       
        timeText.text = $"Betting window closed.\n{resultMessage}";

      
        if (timeUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(timeUpSound);
        }
    }

    // Debug Functions
    private void SkipTime(float seconds)
    {
        betEndTime = betEndTime.AddSeconds(-seconds);
        Debug.Log($"[DEBUG] Skipped {seconds} seconds. New end time: {betEndTime:hh:mm:ss tt}");
    }
}
