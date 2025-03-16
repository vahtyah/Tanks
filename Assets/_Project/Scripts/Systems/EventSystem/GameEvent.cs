public enum GameEventType
{
    //Add state when user need to login or register
    GameAuthentication,
    GameMainMenu,
    GamePreStart,
    GameStart,
    GameRunning,
    GameOver,
    GamePause,
    TogglePause,
}
public struct GameEvent
{
    public GameEventType EventType;

    private GameEvent(GameEventType eventType) { EventType = eventType; }

    public static void Trigger(GameEventType eventType) { EventManager.TriggerEvent(new GameEvent(eventType)); }
}