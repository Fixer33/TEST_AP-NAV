#if !UNITY_ANDROID
using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

public class NetworkServer : MonoBehaviour
{
    public static NetworkServer instance { get; private set; }

    private NetworkDriver _driver;
    private NetworkConnection _connection;

    [SerializeField] private ushort Port = 24842;

    private void Start()
    {
        instance = this;
        StartServer();
    }

#region Shutdown
    private void OnDestroy()
    {
        ShutdownServer();
    }
    private void ShutdownServer()
    {
        if (_driver.IsCreated)
        {
            _driver.Dispose();
            _connection = default;
        }
    } 
#endregion

    private void Update()
    {
        UpdateServer();
    }

    public void SendData(PacketType type, params object[] data)
    {
        _driver.BeginSend(NetworkPipeline.Null, _connection, out var writer);
        NetworkPacketSender.WriteData(ref writer, type, data);
        _driver.EndSend(writer);
    }

    private void StartServer()
    {
        _driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.Parse(GetLocalIp(), Port, NetworkFamily.Ipv4);
        Debug.Log(endpoint.Address);
        if (_driver.Bind(endpoint) != 0)
            Debug.Log("Failed to bind to port " + Port);
        else
            _driver.Listen();

        _connection = default;
    }
    

    public void Disconnect()
    {
        _connection.Disconnect(_driver);
        _driver.Disconnect(_connection);
        _connection = default;
        Network.instance.Disconnected();
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
    private void UpdateServer()
    {
        _driver.ScheduleUpdate().Complete();

        AcceptConnection();
        HandleNetworkData();
    }

    private void AcceptConnection()
    {
        NetworkConnection c;
        while ((c = _driver.Accept()) != default(NetworkConnection))
        {
            if (_connection == default)
            {
                _connection = c;
                Debug.Log("Connection established");
                Network.instance.Connected();
            }
        }
    }

    private void HandleNetworkData()
    {
        CheckConnection();

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = _driver.PopEventForConnection(_connection, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {
                NetworkPacketHandle.Handle(ref stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected");
                Network.instance.Disconnected();
                _connection = default(NetworkConnection);
            }
        }
    }

    private void CheckConnection()
    {
        if (!_connection.IsCreated && Network.instance.IsConnected)
        {
            Debug.Log("Connection lost");
            Network.instance.Disconnected();
            return;
        }
    }
}
#endif