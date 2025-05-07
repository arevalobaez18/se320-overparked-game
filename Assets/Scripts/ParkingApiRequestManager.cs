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
    public void OnDataUpdate();
}

public class ParkingApiRequestManager : MonoBehaviour
{
    private List<IParkingRequestObserver> _observers = new List<IParkingRequestObserver>();
    public JsonResponseParkingStructure[] ParkingStructures { get; private set; } = Array.Empty<JsonResponseParkingStructure>();

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


    [ContextMenu("Test GET request")]
    public void TestGetRequest()
    {
        Start();
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
                    var response = webRequest.downloadHandler.text;
                    ParseSuccessfulJsonResponse(response);
                    break;
            }
        }
    }

    public static object GetRequest(string url, Action<string> onSuccess)
    {
        throw new NotImplementedException();
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
