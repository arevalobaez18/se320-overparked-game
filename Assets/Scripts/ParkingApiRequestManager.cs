using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class JsonResponseParkingStructure
{
    public string address;
    public int capacity;
    public int currentCount;
    public string name;
}

[System.Serializable]
class JsonParkingResponse
{
    public JsonResponseParkingStructure[] parkingStructures;
}

public interface IParkingRequestObserver
{
    public void OnDataUpdate();
}

public class ParkingApiRequestManager : MonoBehaviour
{
    private List<IParkingRequestObserver> _observers = new List<IParkingRequestObserver>();

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

    public JsonResponseParkingStructure[] GetParkingStructures()
    {
        // TODO
        return Array.Empty<JsonResponseParkingStructure>();
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
                    Debug.Log("Response: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}
