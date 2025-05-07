using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
partial class JsonParkingStructure
{
    public string address;
    public int capacity;
    public int currentCount;
    public new string name;
}

[System.Serializable]
class JsonParkingResponse
{
    public JsonParkingStructure[] parkingStructures;
}

public class ParkingApiRequestManager : MonoBehaviour
{
    public JsonParkingStructure[] GetParkingStructures()
    {
        // TODO
        return Array.Empty<JsonParkingStructure>();
    }

    private void Start()
    {
        StartCoroutine(GetRequest("https://webfarm.chapman.edu/parkinginfo/"));
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
