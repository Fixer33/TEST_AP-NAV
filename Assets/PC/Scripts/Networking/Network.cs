#if !UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network instance { get; private set; }

    public bool IsConnected { get; private set; } = false;

    private void Start()
    {
        instance = this;
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