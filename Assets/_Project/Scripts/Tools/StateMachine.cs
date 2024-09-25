using System;
using System.Collections.Generic;
using UnityEngine;
public struct StateMachineEvent<T> where T : struct
{
    public StateMachine<T> StateMachine;
    public StateMachineEvent(StateMachine<T> stateMachine)
    {
        StateMachine = stateMachine;
    }
}

public interface IStateMachine
{
    bool TriggerEvents { get; set; }
}

public class StateMachine<T> : IStateMachine where T: struct
{
    public bool TriggerEvents { get; set; }
    public GameObject Target;
    public T CurrentState { get; protected set; }
    public T PreviousState { get; protected set; }

    public Action OnStateChange;

    public StateMachine(GameObject target, bool triggerEvents = true)
    {
        this.Target = target;
        this.TriggerEvents = triggerEvents;
    }

    public void ChangeState(T newState)
    {
        if (EqualityComparer<T>.Default.Equals(newState, CurrentState))
        {
            return;
        }

        PreviousState = CurrentState;
        CurrentState = newState;

        OnStateChange?.Invoke();

        if (TriggerEvents)
        {
            EventManger.TriggerEvent(new StateMachineEvent<T>(this));
        }
    }
}