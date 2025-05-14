using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;
using System.Net.Http;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;


public class ParkingApiRequestTests
{
    private ParkingApiRequestManager parkingApiRequestManager;
    private bool successCalled;
    private bool errorCalled;
    private string responseText;
    private string errorText;

    [SetUp]
    public void SetUp()
    {
        parkingApiRequestManager = new ParkingApiRequestManager();
        successCalled = false;
        errorCalled = false;
        responseText = "";
        errorText = "";
    }

    [UnityTest]
    public IEnumerator GetRequest_SuccessfulResponse_ReturnsExpectedResult()
    {
        //Arrange
        string url = "https://webfarm.chapman.edu/parkinginfo/";

        yield return ParkingApiRequestManager.GetRequest(url,
            onSuccess: (response) =>
            {
                successCalled = true;
                responseText = response;
            });
        
        //Assert
        Assert.IsTrue(successCalled, "Success callback should be invoked.");
        Assert.IsFalse(errorCalled, "Error callback should not be invoked.");
        Assert.IsNotEmpty(responseText, "Response text should not be empty.");
    }

    [UnityTest]
    public IEnumerator GetRequest_InvalidURL_ReturnsError()
    {
        //Arrange
        string url = "https://invalidurl.test";

        yield return ParkingApiRequestManager.GetRequest(url,
            onSuccess: (response) =>
            {
                successCalled = true;
                responseText = response;
            },
            onError: (error) =>
            {
                errorCalled = true;
                errorText = error;
            }
        );
        
        //Assert
        Assert.IsFalse(successCalled, "Success callback should not be invoked.");
        Assert.IsTrue(errorCalled, "Error callback should be invoked.");
        Assert.IsNotEmpty(errorText, "Error message should not be empty.");
        
        yield return null;
    }
}