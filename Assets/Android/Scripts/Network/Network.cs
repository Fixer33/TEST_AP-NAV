#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network instance { get; private set; }

    public bool IsConnected { get; private set; }

    private List<string> _visibleIpAdresses = new List<string>();

    private void Start()
    {
        instance = this;
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
        print(1);
        _visibleIpAdresses.Clear();
        List<Ping> toPing = new List<Ping>();
        for (int q2 = 168; q2 < 170; q2++)
        {
            List<int> possibleQ3 = new List<int>() { 0, 1, 3, 7, 15, 31, 63, 127, 255 };
            foreach (var q3 in possibleQ3)
            {
                for (int q4 = 1; q4 < 254; q4++)
                {
                    string ip = $"192.{q2}.{q3}.{q4}";
                    toPing.Add(new Ping(ip));
                }
            }
        }
        for (int i = 0; i < 100; i++)
        {
            for (int k = 0; k < toPing.Count; k++)
            {
                Ping p = toPing[k];
                if (p.isDone)
                    print("==");
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
        UI.instance.RefreshDeviceButtons(_visibleIpAdresses);
    }

    public void ConnectionEstablished()
    {
        IsConnected = true;
        UI.instance.SetConnectionPanelVisibility(false);
    }

    public void ConnectionLost()
    {
        IsConnected = false;
        UI.instance.SetConnectionPanelVisibility(true);
    }
}
#endif