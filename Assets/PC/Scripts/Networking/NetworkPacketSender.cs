#if !UNITY_ANDROID
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
        _handlers.Add(PacketType.S_VehiclePosition, new NetworkPacketDelegate(VehiclePosition));
        _handlers.Add(PacketType.S_VehicleFailure, new NetworkPacketDelegate(VehicleFailure));
        _handlers.Add(PacketType.S_ManualInput, new NetworkPacketDelegate(ManualInput));
        _handlers.Add(PacketType.ConnectionCheck, new NetworkPacketDelegate(ConnectionCheckSend));
    }

    private static void ConnectionCheckSend(ref DataStreamWriter writer, object[] data)
    {
        writer.WriteByte(0);
    }

    private static void VehiclePosition(ref DataStreamWriter writer, object[] data)
    {
        if (data.Length != 1)
            return;

        if (data[0] is Vector3 == false)
            return;

        Vector3 v = (Vector3)data[0];

        writer.WriteFloat(v.x);
        writer.WriteFloat(v.y);
        writer.WriteFloat(v.z);
    }

    private static void VehicleFailure(ref DataStreamWriter writer, object[] data)
    {
        if (data.Length != 1)
            return;

        if (data[0] is bool == false)
            return;

        bool failed = (bool)data[0];
        writer.WriteByte(Convert.ToByte(failed));
    }

    private static void ManualInput(ref DataStreamWriter writer, object[] data)
    {
        writer.WriteByte(0);
    }
}
#endif