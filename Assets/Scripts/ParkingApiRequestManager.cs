using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
