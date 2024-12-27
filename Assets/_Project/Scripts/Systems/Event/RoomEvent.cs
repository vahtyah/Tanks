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
    public Player Player;

    public RoomEvent(RoomEventType eventType, Room room = null, Player player = null)
    {
        EventType = eventType;
        Room = room;
        Player = player;
    }
    
    public static void Trigger(RoomEventType eventType,  Player player)
    {
        EventManager.TriggerEvent(new RoomEvent(eventType, null, player));
    }

    public static void Trigger(RoomEventType eventType, Room room)
    {
        EventManager.TriggerEvent(new RoomEvent(eventType, room));
    }
}