public struct NotificationEvent
{
    public string Title { get; private set; }
    public string Destination { get; private set; }
    
    private static NotificationEvent cache;
    
    public static void Trigger(string title, string destination)
    {
        cache.Title = title; 
        cache.Destination = destination;
        EventManager.TriggerEvent(cache);
    }
}