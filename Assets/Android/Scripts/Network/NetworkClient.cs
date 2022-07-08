#if UNITY_ANDROID
using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;

public class NetworkClient : MonoBehaviour
{
    public static NetworkClient instance { get; private set; }

    [SerializeField] private ushort Port = 24842;

    public bool Done { get; private set; }

    private NetworkDriver _driver;
    private NetworkConnection _connection;
    private Network _network;

    private void Start()
    {
        instance = this;
        _network = FindObjectOfType<Network>();
    }

    public void ConnectToServer(string ip)
    {
        _driver = NetworkDriver.Create();
        _connection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.Parse(ip, Port, NetworkFamily.Ipv4);
        _connection = _driver.Connect(endpoint);
        Debug.Log("Connecting to " + endpoint);
    }

    public void OnDestroy()
    {
        _driver.Dispose();
    }

    void Update()
    {
        UpdateServerData();
    }

    private void UpdateServerData()
    {
        if (_connection == default)
        {
            Debug.Log("Connection is null");
            return;
        }

        _driver.ScheduleUpdate().Complete();
        CheckForErrors();
        HandleServerData();
    }

    private void CheckForErrors()
    {
        if (!_connection.IsCreated)
        {
            Debug.Log("Connection error");
            Network.instance.ConnectionLost();
            return;
        }
    }

    private void HandleServerData()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = _connection.PopEvent(_driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Connection established");
                _network.ConnectionEstablished();

                //uint value = 1;
                //_driver.BeginSend(_connection, out var writer);
                //writer.WriteUInt(value);
                //_driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                NetworkPacketHandle.Handle(ref stream);

                //Debug.Log("Got the value = " + value + " back from the server");
                //Done = true;
                //_connection.Disconnect(_driver);
                //_connection = default(NetworkConnection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Connection lost");
                _network.ConnectionLost();
                _connection = default(NetworkConnection);
            }
        }
    }

    public void SendData(PacketType type, params object[] data)
    {
        _driver.BeginSend(NetworkPipeline.Null, _connection, out var writer);
        NetworkPacketSender.WriteData(ref writer, type, data);
        _driver.EndSend(writer);
    }
}
#endif