public enum GameModeType
{
    LocalMatch,
    BotMatch,
}

public struct GameModeEvent
{
    public GameModeType GameModeType;

    private GameModeEvent(GameModeType gameModeType) { GameModeType = gameModeType; }

    public static void Trigger(GameModeType gameModeType) { EventManger.TriggerEvent(new GameModeEvent(gameModeType)); }
}


public enum GameEventType
{
    GameMainMenu,
    GamePreStart,
    GameStart,
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

public enum EventType
{
    PlayerDeath,
    GamePreStart,
    GameStart,
    GamePause,
    GameUnPause,
    GameOver,
    TogglePause,
}

public struct Event
{
    public EventType EventType;
    public PlayerCharacter OriginCharacter;

    public Event(EventType eventType, PlayerCharacter originCharacter)
    {
        EventType = eventType;
        OriginCharacter = originCharacter;
    }

    public static void Trigger(EventType eventType, PlayerCharacter originCharacter)
    {
        EventManger.TriggerEvent(new Event(eventType, originCharacter));
    }
}