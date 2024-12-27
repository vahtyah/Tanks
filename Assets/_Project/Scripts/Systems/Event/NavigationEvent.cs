public enum NavigationType
{
    Home,
    Multiplayer,
    Settings,
    Quit,
    Graphics,
    Audio,
}

public struct NavigationEvent
{
    public NavigationType NavigationType;
    private static NavigationEvent cacheEvent;
    
    public NavigationEvent(NavigationType navigationType)
    {
        NavigationType = navigationType;
    }
    
    public static void Trigger(NavigationType navigationType)
    {
        cacheEvent.NavigationType = navigationType;
        EventManager.TriggerEvent(cacheEvent);
    }
}