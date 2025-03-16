using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IEventListenerBase
{
};

public interface IEventListener<T> : IEventListenerBase
{
    void OnEvent(T e);
}

public static class EventManager
{
    private static readonly Dictionary<Type, HashSet<IEventListenerBase>> subscribers = new();

    public static void AddListener<T>(IEventListener<T> listener) where T : struct
    {
        Type eventType = typeof(T);
        if (!subscribers.TryGetValue(eventType, out var subs))
        {
            subs = new HashSet<IEventListenerBase>();
            subscribers[eventType] = subs;
        }

        subs.Add(listener);
    }

    public static void RemoveListener<T>(IEventListener<T> listener) where T : struct
    {
        Type eventType = typeof(T);
        if (!subscribers.TryGetValue(eventType, out var subs)) return;
        if (subs.Remove(listener) && subs.Count == 0)
        {
            subscribers.Remove(eventType);
        }
    }

    private static bool SubscriptionExists<T>(Type eventType, IEventListener<T> listener) where T : struct
    {
        return subscribers[eventType].Contains(listener);
    }

    public static void TriggerEvent<T>(T e) where T : struct
    {
        Type eventType = typeof(T);
        if (!subscribers.TryGetValue(eventType, out var subs)) return;
        HashSet<IEventListenerBase> subsCopy = new HashSet<IEventListenerBase>(subs);
        foreach (var listenerBase in subsCopy)
        {
            if (listenerBase is IEventListener<T> listener)
            {
                listener.OnEvent(e);
            }
        }
    }
}

public static class EventExtensions
{
    public static void StartListening<T>(this IEventListener<T> listener) where T : struct
    {
        EventManager.AddListener(listener);
    }

    public static void StopListening<T>(this IEventListener<T> listener) where T : struct
    {
        EventManager.RemoveListener(listener);
    }
}

// public enum GameEventType
// {
//     GameMainMenu,
//     GamePreStart,
//     GameStart,
//     GameOver,
//     GamePause,
// }
//
// public struct GameEvent
// {
//     public GameEventType EventType;
//     public GameObject Canvas;
//     
//     public GameEvent(GameEventType eventType, GameObject canvas)
//     {
//         EventType = eventType;
//         Canvas = canvas;
//     }
//     
//     public static void Trigger(GameEventType eventType, GameObject canvas)
//     {
//         EventManger.TriggerEvent(new GameEvent(eventType, canvas));
//     }
// }

//How to use:
//1. Create an event class that implements IEventListenerBase
//2. Create a listener class that implements IEventListener<T> where T is the event class
//3. Call EventRegister.StartListening(this) in the listener's OnEnable method
// void OnEnable()
// {
// 	this.StartListening<MMGameEvent>();
// }
//4. Call EventRegister.StopListening(this) in the listener's OnDisable method
// void OnDisable()
// {
// 	this.StopListening<MMGameEvent>();
// }
//5. Implement the EventListener interface for that event. For example :
// public void OnEvent(GameEvent gameEvent)
// {
// 	if (gameEvent.EventName == "GameOver")
//		{
//			// DO SOMETHING
//		}
// } 