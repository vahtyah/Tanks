public enum ButtonType
{
    Home,
    Multiplayer,
    Settings,
    Quit,
    Graphics,
    Audio,
}

public struct ButtonEvent
{
    public ButtonType ButtonType;
    private static ButtonEvent cacheEvent;
    
    public ButtonEvent(ButtonType buttonType)
    {
        ButtonType = buttonType;
    }
    
    public static void Trigger(ButtonType buttonType)
    {
        cacheEvent.ButtonType = buttonType;
        EventManger.TriggerEvent(cacheEvent);
    }
}