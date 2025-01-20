using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private int batteryCount = 0;
    private int wireCount = 0;

    public void IncreaseBatteryCount()
    {
        batteryCount++;
        Debug.Log($"Battery count increased: {batteryCount}");
    }

    public void IncreaseWireCount()
    {
        wireCount++;
        Debug.Log($"Wire count increased: {wireCount}");
    }
}