using System.Collections.Generic;

public enum FriendEventType
{
    RequestSent,
    RequestAccepted,
    RequestRejected,
    FriendRemoved,
    FriendsLoaded
}

public struct FriendEvent
{
    public FriendEventType EventType;
    public string DisplayName;
    public List<FriendData> FriendsList;
    
    private static FriendEvent cache;
    
    public static void Trigger(FriendEventType eventType, string displayName)
    {
        cache.EventType = eventType;
        cache.DisplayName = displayName;
        EventManager.TriggerEvent(cache);
    }
}