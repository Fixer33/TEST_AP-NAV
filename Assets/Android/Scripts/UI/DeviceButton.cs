#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DeviceButton : MonoBehaviour
{
    [SerializeField] private Text ButtonText;
    [SerializeField] private string Ip;

    public void SetIp(string ip)
    {
        if (Regex.Match(ip, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Success == false)
            return;

        Ip = ip;
        ButtonText.text = Ip;
    }

    public void Connect()
    {
        Network.instance.Connect(Ip);
    }
}
#endif