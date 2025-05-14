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
    private const String URL = "https://webfarm.chapman.edu/ParkingService/ParkingService/counts";
    private const float SecondsToWaitBetweenRequests = 5;

    private List<IParkingRequestObserver> _observers = new List<IParkingRequestObserver>();
    public JsonResponseParkingStructure[] ParkingStructures { get; private set; } = Array.Empty<JsonResponseParkingStructure>();

    public bool updateLoopIsRunning = true;

    [CanBeNull] private IEnumerator routine;

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

    private void ParseSuccessfulJsonResponse(string response)
    {
        JsonParkingResponse parkingResponse = JsonUtility.FromJson<JsonParkingResponse>(response);
        ParkingStructures = parkingResponse.Structures;
        Debug.Log(ParkingStructures.Length + " parking structures stored from API");
        CallObservers();
    }

    private void CallObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnDataUpdate();
        }
    }
}
