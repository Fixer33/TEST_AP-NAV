#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network instance { get; private set; }

    public bool IsConnected { get; private set; }

    private List<string> _visibleIpAdresses = new List<string>();

    private int _framesSinceLastConnectionCheck = 0;
    private bool _connectionCheckActive = false;

    private void Start()
    {
        instance = this;
        RefreshDevices();
    }

    private void Update()
    {
        if (_connectionCheckActive)
        {
            if (IsConnected && _framesSinceLastConnectionCheck++ > 100)
            {
                NetworkClient.instance.Disconnect();
            }
        }
        else
        {
            if (Time.frameCount % 45 == 0)
            {
                _framesSinceLastConnectionCheck = 0;
                NetworkClient.instance.SendData(PacketType.ConnectionCheck);
            }
        }
        if (_visibleIpAdresses.Count > 0 && UI.instance.DeviceButtonsCount != _visibleIpAdresses.Count)
        {
            UI.instance.RefreshDeviceButtons(_visibleIpAdresses);
        }
    }

    public void ConnectionCheckRecieved()
    {
        _framesSinceLastConnectionCheck = 0;
        _connectionCheckActive = false;
        NetworkClient.instance.SendData(PacketType.ConnectionCheck);
    }

    public void Connect(string validIp)
    {
        if (IsConnected == false)
        {
            FindObjectOfType<NetworkClient>().ConnectToServer(validIp);
        }
    }

    public void RefreshDevices()
    {
        StartCoroutine(RefreshAvaibleIpList());
    }

    private IEnumerator RefreshAvaibleIpList()
    {
        _visibleIpAdresses.Clear();
        string localIpSelf = GetLocalIp();
        List<Ping> toPing = new List<Ping>();

        var matches = Regex.Match(localIpSelf, @"192\.168\.(\d{1,3})\.(\d{1,3})");
        string q3 = matches.Groups[1].Value;
        int q4 = int.Parse(matches.Groups[2].Value);
        int min = q4 - 20;
        int max = q4 + 20;
        if (min < 1)
            min = 1;
        if (max > 254)
            max = 254;

        for (int i = min; i < max; i++)
        {
            string ipToCheck = $"192.168.{q3}.{i}";
            if (ipToCheck == localIpSelf)
            {
                _visibleIpAdresses.Add(localIpSelf);
                continue;
            }
            Ping p = new Ping(ipToCheck);
            toPing.Add(p);
        }

        for (int i = 0; i < 100; i++)
        {
            for (int k = 0; k < toPing.Count; k++)
            {
                Ping p = toPing[k];
                if (p.isDone && _visibleIpAdresses.Contains(p.ip) == false)
                    _visibleIpAdresses.Add(p.ip);
            }
            yield return new WaitForSeconds(0.05f);
        }
        for (int k = 0; k < toPing.Count; k++)
        {
            Ping p = toPing[k];
            p.DestroyPing();
        }
        toPing.Clear();
        yield return new WaitForSeconds(0.05f);
    }
    private string GetLocalIp()
    {
        string hostName = Dns.GetHostName();
        var host = Dns.GetHostByName(hostName);
        for (int i = 0; i < host.AddressList.Length; i++)
        {
            string adress = host.AddressList[i].ToString();
            if (Regex.Match(adress, @"\b192\.168\.\d{1,3}\.\d{1,3}\b").Success)
                return adress;
        }
        return "127.0.0.1";
    }

    public void ConnectionEstablished()
    {
        IsConnected = true;
        NetworkClient.instance.SendData(PacketType.ConnectionCheck);
        _connectionCheckActive = true;
        UI.instance.SetConnectionPanelVisibility(false);
    }

    public void ConnectionLost()
    {
        IsConnected = false;
        _connectionCheckActive = false;
        UI.instance.SetConnectionPanelVisibility(true);
    }
}
#endif