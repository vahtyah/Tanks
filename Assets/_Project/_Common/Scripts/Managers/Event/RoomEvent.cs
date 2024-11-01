using Photon.Realtime;

public enum RoomEventType
{
    RoomCreate,
    RoomJoin,
    RoomLeave,
    PlayerJoin,
    PlayerLeft,
}

public struct RoomEvent
{
    public RoomEventType EventType;
    public Room Room;

    public RoomEvent(RoomEventType eventType, Room room)
    {
        EventType = eventType;
        Room = room;
    }

    public static void Trigger(RoomEventType eventType, Room room)
    {
        EventManger.TriggerEvent(new RoomEvent(eventType, room));
    }
}