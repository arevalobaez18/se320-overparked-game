using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrencyManager : MonoBehaviour, IParkingRequestObserver
{
    public static CurrencyManager Instance;

    public int currency;
    public Text currencyText;

    private int currentBetAmount = 10; 
    private int previousCapacityDifference = 0;  

    private float updateInterval = 1f;
    private float lastUpdateTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        PlayerPrefs.DeleteKey("Currency");
        PlayerPrefs.Save();

        LoadCurrency();
    }

    private void Start()
    {
        UpdateCurrencyUI();
        ParkingApiRequestManager.Instance.AddObserver(this);
    }

    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateCurrencyUI();
            lastUpdateTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Add10();
        }
    }

    // New method to handle parking capacity changes
    public void OnParkingCapacityChanged(int capacityDifference)
    {
        previousCapacityDifference = capacityDifference;

    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        Debug.Log($"[AddCurrency] +{amount}, total = {currency}");
        UpdateCurrencyUI();
        SaveCurrency();
    }

    public void SubtractCurrency(int amount)
    {
        if (currency > amount)
        {
            currency -= amount;
            Debug.Log($"[SubtractCurrency] -{amount}, total = {currency}");
            UpdateCurrencyUI();
            SaveCurrency();
        }
        else
        {
            Debug.LogWarning("Not enough currency!");
            currency = 0;
            Debug.Log("[CurrencyManager] Money is zero, adding 10 currency.");
            Invoke("Add10", 1f); 
            
            
        }

    }
    public void Add10()
    {
        AddCurrency(10);
        print("add10");

    }
    public void OnOptionButtonClicked() 
    {
        SubtractCurrency(currentBetAmount);  // Deduct 10 currency for the bet
        StartCoroutine(ParkPrediction());  
    }

    public void GuessHigher()
    {
        SubtractCurrency(currentBetAmount);
        StartCoroutine(HigherPrediction());
        
        
    }
    private IEnumerator HigherPrediction()
    {
        int previousTotalCapacity = ParkingApiRequestManager.Instance.GetTotalParkingCapacity();

        yield return new WaitForSeconds(2f);  // Assume 2 seconds later, parking data updates

        int newTotalCapacity = ParkingApiRequestManager.Instance.GetTotalParkingCapacity();

        if (newTotalCapacity > previousTotalCapacity)
        {
            int reward = currentBetAmount * 2;  // Double the bet as reward if correct
            AddCurrency(reward);
            Debug.Log($"[ParkPrediction] Correct guess! Rewarded {reward} currency.");
        }

    }


    public void GuessLower()
    {
        SubtractCurrency(currentBetAmount);
        StartCoroutine(LowerPrediction());


    }
    private IEnumerator LowerPrediction()
    {
        int previousTotalCapacity = ParkingApiRequestManager.Instance.GetTotalParkingCapacity();

        yield return new WaitForSeconds(2f);  // Assume 2 seconds later, parking data updates

        int newTotalCapacity = ParkingApiRequestManager.Instance.GetTotalParkingCapacity();

        if (newTotalCapacity < previousTotalCapacity)
        {
            int reward = currentBetAmount * 2;  // Double the bet as reward if correct
            AddCurrency(reward);
            Debug.Log($"[ParkPrediction] Correct guess! Rewarded {reward} currency.");
        }

    }

    // New method to handle the betting logic
    private IEnumerator ParkPrediction()
    {
        int previousTotalCapacity = ParkingApiRequestManager.Instance.GetTotalParkingCapacity();

        yield return new WaitForSeconds(2f);  // Assume 2 seconds later, parking data updates

        int newTotalCapacity = ParkingApiRequestManager.Instance.GetTotalParkingCapacity();

        bool guessedHigher = (newTotalCapacity > previousTotalCapacity);

        if (guessedHigher)  // Check if the guess is correct
        {
            int reward = currentBetAmount * 2;  // Double the bet as reward if correct
            AddCurrency(reward);
            Debug.Log($"[ParkPrediction] Correct guess! Rewarded {reward} currency.");
        }
        else  // If the guess is incorrect
        {
            Debug.Log("[ParkPrediction] Incorrect guess, lost the bet.");
        }
    }

    void UpdateCurrencyUI()
    {
        if (currencyText != null)
        {
            currencyText.text = "Currency: " + currency.ToString();
        }
    }

    void SaveCurrency()
    {
        PlayerPrefs.SetInt("Currency", currency);
        PlayerPrefs.Save();
    }

    void LoadCurrency()
    {
        currency = PlayerPrefs.GetInt("Currency", 100);
        Debug.Log($"[LoadCurrency] Loaded currency: {currency}");
    }

    public void OnDataUpdate()
    {
        Debug.Log("[CurrencyManager] Received updated parking data");
    }

    void OnDestroy()
    {
        if (ParkingApiRequestManager.Instance != null)
        {
            ParkingApiRequestManager.Instance.RemoveObserver(this);
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("[CurrencyManager] Game exiting, clearing data");
        PlayerPrefs.DeleteKey("Currency");
        PlayerPrefs.Save();
    }
}
