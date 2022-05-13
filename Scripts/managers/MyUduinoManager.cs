using System;
using System.Collections;
using Uduino;
using UnityEngine;

public class MyUduinoManager : Singleton<MyUduinoManager>
{
    public bool IsConnected { get; set; }
    public int HR { get; private set; }

    void Start()
    {
        UduinoManager.Instance.OnBoardConnected += OnBoardConnected; //Create the Delegate
        UduinoManager.Instance.OnDataReceived += OnDataReceived;
    }

    private void OnDataReceived(string data, UduinoDevice device)
    {
        try
        {
            HR = Int32.Parse(data);
        }
        catch (FormatException)
        {
            HR = 60;
        }
    }

    void Stuart()
    {
        // UduinoManager.Instance.pinMode(blinkPin, PinMode.Output);

        //To send data to uduino, command name must be same.
        //UduinoManager.Instance.sendCommand("myNodemcu", "myCommand", 10, 3);
    }

    public void OnBoardConnected(UduinoDevice deviceName)
    {
        IsConnected = true;
    }

    public void BoardDisconnected(UduinoDevice device)
    {
        print("Event: Board " + device.name + " disconnected.");
        IsConnected = false;
    }

}