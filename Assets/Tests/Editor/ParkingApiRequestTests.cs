using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

public class ParkingApiRequestTests
{
    private ParkingApiRequestManager _manager;

    private class MockObserver : IParkingRequestObserver
    {
        public bool WasCalled = false;

        public void OnDataUpdate()
        {
            WasCalled = true;
        }
    }
    
    [SetUp]
    public void SetUp()
    {
        _manager = new GameObject().AddComponent<ParkingApiRequestManager>();
    }

    
    
    [Test]
    public void AddObserver_RegisterObserverSuccessfully()
    {
        var observer = new MockObserver();
        _manager.AddObserver(observer);

        _manager.TestStartGettingUpdates();

        Assert.IsTrue(_manager.updateLoopIsRunning);
    }

    [Test]
    public void RemoveObserver_RemoveObserverSuccessfully()
    {
        var observer = new MockObserver();
        _manager.AddObserver(observer);
        _manager.RemoveObserver(observer);
    }

    [Test]
    public void GetAPIPercentage_ReturnsCorrectPercentage()
    {
        _manager.SetParkingStructuresForTest(new[] 
        {
            new JsonResponseParkingStructure
            {
                Name = "Test Structure",
                Capacity = 100,
                CurrentCount = 50,
                Address = "123 St"
            }
        });
    }
    
}