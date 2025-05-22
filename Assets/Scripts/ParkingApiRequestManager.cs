using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class JsonResponseParkingStructure
{
    public string Address;
    public int Capacity;
    public int CurrentCount;
    public string Name;
}

[System.Serializable]
class JsonParkingResponse
{
    public JsonResponseParkingStructure[] Structures;
}

public interface IParkingRequestObserver
{
    void OnDataUpdate();
}

public class ParkingApiRequestManager : MonoBehaviour
{
    public static ParkingApiRequestManager Instance { get; private set; }

    private int previousTotalCapacity = 0; // To store previous total parking capacity

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private List<IParkingRequestObserver> _observers = new List<IParkingRequestObserver>();
    public JsonResponseParkingStructure[] ParkingStructures { get; private set; } = Array.Empty<JsonResponseParkingStructure>();

    public void AddObserver(IParkingRequestObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IParkingRequestObserver observer)
    {
        _observers.Remove(observer);
    }

    private void Start()
    {
        StartCoroutine(GetRequest("https://webfarm.chapman.edu/ParkingService/ParkingService/counts"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    print("Test");
                    var response = webRequest.downloadHandler.text;
                    print(response);
                    ParseSuccessfulJsonResponse(response);
                    break;
            }
        }
    }

    private void ParseSuccessfulJsonResponse(string response)
    {
        JsonParkingResponse parkingResponse = JsonUtility.FromJson<JsonParkingResponse>(response);
        ParkingStructures = parkingResponse.Structures;
        Debug.Log($"[ParkingApi] {ParkingStructures.Length} structures received from API");

        // Compare current total capacity with previous one
        int currentTotalCapacity = GetTotalParkingCapacity();
        int capacityDifference = currentTotalCapacity - previousTotalCapacity;

        // Notify observers about the data update and the change in capacity
        CallObservers(capacityDifference);

        // Update previous capacity for the next comparison
        previousTotalCapacity = currentTotalCapacity;
    }


    private void CallObservers(int capacityDifference)
    {
        foreach (var observer in _observers)
        {
            observer.OnDataUpdate();  // Notifying that the data has updated

            // Optionally, you could pass the capacity difference to the observer, e.g.:
            if (observer is CurrencyManager currencyManager)
            {
                currencyManager.OnParkingCapacityChanged(capacityDifference);  // This is a new method you can create in CurrencyManager
            }
        }
    }

    public int GetTotalParkingCapacity()
    {
        int total = 0;
        foreach (var structure in ParkingStructures)
        {
            total += structure.Capacity;
        }
        print("total:"+total);
        return total;
    }
}
