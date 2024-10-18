public enum GameEventType
{
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

    public static void Trigger(GameEventType eventType) { EventManger.TriggerEvent(new GameEvent(eventType)); }
}

public enum CharacterEventType
{
    CharacterSpawn,
    CharacterDeath,
    CharacterHit,
}

public struct CharacterEvent
{
    public CharacterEventType EventType;
    public Character Character;

    public CharacterEvent(CharacterEventType eventType, Character character)
    {
        EventType = eventType;
        Character = character;
    }

    public static void Trigger(CharacterEventType eventType, Character character)
    {
        EventManger.TriggerEvent(new CharacterEvent(eventType, character));
    }
}