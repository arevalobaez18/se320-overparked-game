using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrencyManager : MonoBehaviour, IParkingRequestObserver
{
    public static CurrencyManager Instance;

    public int currency;
    public Text currencyText;
    public Text countdownText;
    public GameObject resultPanel;
    public Text resultText;

    private int currentBetAmount = 10;  // Default bet amount
    private int previousCapacityDifference = 0;  // Store the previous capacity change for betting logic

    private float updateInterval = 1f;
    private float lastUpdateTime;

    public GameObject loggedBetPrefab;
    public Transform betLogContentParent;

    private float countdownTime = 300f;  // 5-minute countdown

    public ICurrencyDisplayStrategy currencyDisplayStrategy = new DollarSignCurrencyDisplayStrategy();

    public void LogBet(int amount, bool isHigher, string structureName)
    {
        if (loggedBetPrefab == null || betLogContentParent == null)
        {
            Debug.LogWarning("LoggedBet prefab or content parent not assigned!");
            return;
        }
        var betObj = Instantiate(loggedBetPrefab, betLogContentParent);
        var loggedBet = betObj.GetComponent<LoggedBet>();
        if (loggedBet != null)
            loggedBet.SetBetInfo(amount, isHigher, structureName);
    }

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

        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime; // Decrease countdown time each frame
            UpdateCountdownUI();  // Update the countdown UI
        }
        else
        {
            EndGame();  // End the game when countdown reaches zero
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Add10();
            Debug.Log("[CurrencyManager] F2 pressed and gave +10 currency");
        }
    }

    void UpdateCountdownUI()
    {
        int minutes = Mathf.FloorToInt(countdownTime / 60);  // Get minutes
        int seconds = Mathf.FloorToInt(countdownTime % 60);  // Get seconds
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);  // Format the countdown display
    }

    public void EndGame()
    {
        ShowResultPanel();  // Show the result panel when time is up
    }

    public void ShowResultPanel()
    {
        resultPanel.SetActive(true);  // Activate the result panel
        string resultMessage = (currency >= 100) ? $"You won {currency - 100} currency!" : "You lost the bet!";
        resultText.text = resultMessage;  // Display the result message
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
            Debug.Log("[CurrencyManager] Currency is zero, adding 10 currency.");
            Invoke("Add10", 1f);  // Automatically add 10 currency if balance reaches zero
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
        StartCoroutine(ParkPrediction());  // Start the bet prediction logic
    }
    public void OnParkingCapacityChanged(int capacityDifference)
    {
        previousCapacityDifference = capacityDifference;
        Debug.Log($"Parking capacity changed by: {capacityDifference}");
    }

    public void GuessHigher(string structureName)
    {
        SubtractCurrency(currentBetAmount);
        LogBet(currentBetAmount, true, structureName);
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


    public void GuessLower(string structureName)
    {
        SubtractCurrency(currentBetAmount);
        LogBet(currentBetAmount, false, structureName);
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
            currencyText.text = currencyDisplayStrategy.Display(currency);
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
