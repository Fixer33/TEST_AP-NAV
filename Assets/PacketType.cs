public enum PacketType : uint
{
    S_VehiclePosition = 0,
    S_VehicleFailure = 1,
    C_VehicleSignal = 2,
    C_Autopilot = 3,
    S_ManualInput = 4,
    C_RoutePointsSend = 5,
    ConnectionCheck = 6,

    other
}