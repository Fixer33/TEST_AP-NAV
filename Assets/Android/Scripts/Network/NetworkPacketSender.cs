#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public static class NetworkPacketSender
{
    private static Hashtable _handlers = new Hashtable();

    private delegate void NetworkPacketDelegate(ref DataStreamWriter writer, object[] data);
    
    public static void WriteData(ref DataStreamWriter writer, PacketType type, params object[] data)
    {
        object handlerObj = _handlers[type];
        if (handlerObj == null)
            return;
        NetworkPacketDelegate handler = handlerObj as NetworkPacketDelegate;
        writer.WriteUInt((uint)type);
        handler.Invoke(ref writer, data);
    } 

    static NetworkPacketSender()
    {
        _handlers.Add(PacketType.C_Autopilot, new NetworkPacketDelegate(ActivateAutopilot));
        _handlers.Add(PacketType.C_VehicleSignal, new NetworkPacketDelegate(ToggleVehicleSignal));
        _handlers.Add(PacketType.C_RoutePointsSend, new NetworkPacketDelegate(RoutePointsPositionSend));
        _handlers.Add(PacketType.ConnectionCheck, new NetworkPacketDelegate(ConnectionCheckSend));
    }

    private static void ConnectionCheckSend(ref DataStreamWriter writer, object[] data)
    {
        writer.WriteByte(0);
    }

    private static void ActivateAutopilot(ref DataStreamWriter writer, object[] data)
    {
        if (data.Length != 1)
            return;

        if (data[0] is bool == false)
            return;

        bool apActive = (bool)data[0];
        writer.WriteByte(Convert.ToByte(apActive));
    }

    private static void ToggleVehicleSignal(ref DataStreamWriter writer, object[] data)
    {
        if (data.Length != 1)
            return;

        if (data[0] is bool == false)
            return;

        bool signalActive = (bool)data[0];
        writer.WriteByte(Convert.ToByte(signalActive));
    }

    private static void RoutePointsPositionSend(ref DataStreamWriter writer, object[] data)
    {
        if (data.Length < 4)
            return;

        int length = (int)data[0];
        writer.WriteInt(length);
        for (int i = 1; i < length + 1; i++)
        {
            Vector2 pos = new Vector2((float)data[(i * 2 - 2) + 1] * Navigator.instance.MapScaleMultiplier, (float)data[(i * 2 - 1) + 1] * Navigator.instance.MapScaleMultiplier);
            writer.WriteFloat(pos.x);
            writer.WriteFloat(pos.y);
        }
    }
}
#endif