using Photon.Realtime;

public enum CharacterEventType
{
    CharacterSpawned,
    CharacterDeath,
    CharacterHit
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

public enum InGameEventType
{
    SomeoneDied,
}

public struct InGameEvent
{
    public InGameEventType EventType;
    public Character killer;
    public Character victim;

    public InGameEvent(InGameEventType eventType, Character killer, Character victim)
    {
        EventType = eventType;
        this.killer = killer;
        this.victim = victim;
    }

    public static void Trigger(InGameEventType eventType, Character killer, Character victim)
    {
        EventManger.TriggerEvent(new InGameEvent(eventType, killer, victim));
    }
}