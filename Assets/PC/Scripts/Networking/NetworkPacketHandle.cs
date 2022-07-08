#if !UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public static class NetworkPacketHandle
{
    private static Hashtable _handlers = new Hashtable();

    private delegate void NetworkPacketDelegate(ref DataStreamReader reader);

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
        _handlers.Add(PacketType.C_Autopilot, new NetworkPacketDelegate(AutopilotChange));
        _handlers.Add(PacketType.C_VehicleSignal, new NetworkPacketDelegate(VehicleSignalChange));
        _handlers.Add(PacketType.C_RoutePointsSend, new NetworkPacketDelegate(RoutePointsPositions));
        _handlers.Add(PacketType.ConnectionCheck, new NetworkPacketDelegate(ConnectionCheckRecieved));
    }

    private static void ConnectionCheckRecieved(ref DataStreamReader stream)
    {
        Network.instance.ConnectionCheckRecieved();
    }

    private static void AutopilotChange(ref DataStreamReader stream)
    {
        bool apActive = Convert.ToBoolean(stream.ReadByte());
        Autopilot.instance.SetState(apActive);
    }

    private static void VehicleSignalChange(ref DataStreamReader stream)
    {
        bool signalActive = Convert.ToBoolean(stream.ReadByte());
        Vehicle.instance.ChangeSignalState(signalActive);
    }

    private static void RoutePointsPositions(ref DataStreamReader stream)
    {
        int length = stream.ReadInt();
        Vector2[] positions = new Vector2[length];
        for (int i = 1; i < length + 1; i++)
        {
            positions[i - 1] = new Vector2(stream.ReadFloat(), stream.ReadFloat());
        }
        Autopilot.instance.SetRoutePoints(positions);
    }
}
#endif