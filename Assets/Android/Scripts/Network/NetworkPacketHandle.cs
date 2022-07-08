#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public static class NetworkPacketHandle
{
    private static Hashtable _handlers = new Hashtable();

    private delegate void NetworkPacketDelegate(ref DataStreamReader reader);
    private static Navigator _navigator;

    public static void Handle(ref DataStreamReader stream)
    {
        uint packetType = stream.ReadUInt();
        PacketType type = (PacketType)packetType;
        if (type == PacketType.other)
            return;

        object handlerObj = _handlers[type];
        if (handlerObj == null)
            return;
        NetworkPacketDelegate handler = handlerObj as NetworkPacketDelegate;
        handler.Invoke(ref stream);
    }

    static NetworkPacketHandle()
    {
        _handlers.Add(PacketType.S_VehiclePosition, new NetworkPacketDelegate(VehiclePosition));
        _handlers.Add(PacketType.S_VehicleFailure, new NetworkPacketDelegate(VehicleFailure));
        _handlers.Add(PacketType.S_ManualInput, new NetworkPacketDelegate(ManualInput));
        _handlers.Add(PacketType.ConnectionCheck, new NetworkPacketDelegate(ConnectionCheckRecieved));
    }

    private static void ConnectionCheckRecieved(ref DataStreamReader stream)
    {
        Network.instance.ConnectionCheckRecieved();
    }

    private static void VehiclePosition(ref DataStreamReader stream)
    {
        float x = stream.ReadFloat();
        float y = stream.ReadFloat();
        float z = stream.ReadFloat();

        if (_navigator == null)
            _navigator = Navigator.instance;

        _navigator.UpdateVehiclePosition(new Vector3(x, y, z));
    }

    private static void VehicleFailure(ref DataStreamReader stream)
    {
        bool isFailed = stream.ReadByte() == 1;
        UI.instance.SetFailurePanelVisibility(isFailed);
    }

    private static void ManualInput(ref DataStreamReader stream)
    {
        UI.instance.AutopilotChange(false);
    }
}
#endif