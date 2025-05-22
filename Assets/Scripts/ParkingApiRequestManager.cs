using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    public void OnDataUpdate();
}

public class ParkingApiRequestManager : MonoBehaviour
{
    public static ParkingApiRequestManager Instance { get; private set; }
    private int previousTotalCapacity = 0; // To store previous total parking capacity
    private const String URL = "https://webfarm.chapman.edu/ParkingService/ParkingService/counts";
    private const float SecondsToWaitBetweenRequests = 5;

    private List<IParkingRequestObserver> _observers = new List<IParkingRequestObserver>();
    public JsonResponseParkingStructure[] ParkingStructures { get; private set; } = Array.Empty<JsonResponseParkingStructure>();

    public bool updateLoopIsRunning = true;

    [CanBeNull] private IEnumerator routine;

    // Instantiating a singleton in Unity is different than regular C#
    // See https://gamedevbeginner.com/singletons-in-unity-the-right-way/
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void SetParkingStructuresForTest(JsonResponseParkingStructure[] structures)
    {
        ParkingStructures = structures;
    }

    public void AddObserver(IParkingRequestObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IParkingRequestObserver observer)
    {
        var indexToRemove = -1;

        for (int i = 0; i < _observers.Count; ++i)
        {
            if (_observers[i].Equals(observer))
            {
                indexToRemove = i;
                break;
            }
        }

        _observers.RemoveAt(indexToRemove);
    }


    [ContextMenu("Test start getting updates")]
    public void TestStartGettingUpdates()
    {
        StartGettingUpdates();
    }

    [ContextMenu("Test stop getting updates")]
    public void TestStopGettingUpdates()
    {
        StopGettingUpdates();
    }

    public void StartGettingUpdates()
    {
        routine = GetRequest(URL);
        StartCoroutine(routine);

        updateLoopIsRunning = true;
    }

    public void StopGettingUpdates()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        updateLoopIsRunning = false;
    }

    /// Returns the percentage of available spots for a parking structure by name.
    /// Returns -1 if the structure is not found or has zero capacity.
    public float GetAPIPercentage(string structureName)
    {
        foreach (var structure in ParkingStructures)
        {
            if (structure.Name.Equals(structureName, StringComparison.OrdinalIgnoreCase))
            {
                if (structure.Capacity == 0) return -1f;
                float available = structure.Capacity - structure.CurrentCount;
                return 100f - ((available / structure.Capacity) * 100f);
            }
        }
        Debug.LogWarning($"Structure '{structureName}' not found. Available names: " +
            string.Join(", ", System.Array.ConvertAll(ParkingStructures, s => s.Name)));
        return -1f;
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
                    var response = webRequest.downloadHandler.text;
                    ParseSuccessfulJsonResponse(response);
                    break;
            }

            if (!updateLoopIsRunning) yield break;

            // Note: WaitForSeconds only works when the game is running
            yield return new WaitForSeconds(SecondsToWaitBetweenRequests);
            routine = GetRequest(URL);
            StartCoroutine(routine);
        }
    }

    public static object GetRequest(string url, Action<string> onSuccess)
    {
        throw new NotImplementedException();
    }

    private void ParseSuccessfulJsonResponse(string response)
    {
        Debug.Log("Raw API JSON: " + response);
        JsonParkingResponse parkingResponse = JsonUtility.FromJson<JsonParkingResponse>(response);
        ParkingStructures = parkingResponse.Structures;
        Debug.Log(ParkingStructures.Length + " parking structures stored from API");
        
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
        // e.g. attach an observer to update the UI
        foreach (var observer in _observers)
        {
            observer.OnDataUpdate();

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
        print("total:" + total);
        return total;
    }
}
