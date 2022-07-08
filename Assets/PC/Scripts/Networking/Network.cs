#if !UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network instance { get; private set; }

    public bool IsConnected { get; private set; } = false;

    private int _framesSinceLastConnectionCheck = 0;
    private int _checkRate = 45;

    private void Start()
    {
        instance = this;
        
    }
    private void Update()
    {
        
            if (IsConnected && _framesSinceLastConnectionCheck++ > 100)
            {
                NetworkServer.instance.Disconnect();
            }
    }
    public void ConnectionCheckRecieved()
    {
        _framesSinceLastConnectionCheck = 0;
        NetworkServer.instance.SendData(PacketType.ConnectionCheck);
    }
    public void Connected()
    {
        IsConnected = true;
        Debug.Log(IsConnected);
        GameManager.instance.StartGame();
    }

    public void Disconnected()
    {
        IsConnected = false;
        GameManager.instance.StopGame();
    }
}
#endif