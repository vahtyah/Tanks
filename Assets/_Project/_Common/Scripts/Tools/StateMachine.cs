using System;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    None,
    Once,
    Repeat,
}

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
    TriggerType TriggerType { get; set; }
}

public class StateMachine<T> : IStateMachine where T : struct
{
    public TriggerType TriggerType { get; set; }
    public GameObject Target;
    public T CurrentState { get; protected set; }
    public T PreviousState { get; protected set; }

    public Action<object> OnStateChange;

    public StateMachine(GameObject target, TriggerType triggerType = TriggerType.None)
    {
        this.Target = target;
        this.TriggerType = triggerType;
    }

    public virtual void ChangeState(T newState, object param = null)
    {
        if (EqualityComparer<T>.Default.Equals(newState, CurrentState))
        {
            return;
        }

        PreviousState = CurrentState;
        CurrentState = newState;

        OnStateChange?.Invoke(param);

        if (TriggerType == TriggerType.None)
        {
            return;
        }
        if (TriggerType == TriggerType.Once)
        {
            EventManger.TriggerEvent(new StateMachineEvent<T>(this));
            TriggerType = TriggerType.None;
        }
        else
        {
            EventManger.TriggerEvent(new StateMachineEvent<T>(this));
        }
    }
}

public enum WeaponState
{
    Initializing,
    Ready,
    Reloading,
    Firing,
}

// public struct WeaponStateEvent
// {
//     public WeaponState State;
//     public int Param;
//
//     public WeaponStateEvent(WeaponState state, int param)
//     {
//         State = state;
//         Param = param;
//     }
//
//     public static void TriggerEvent(WeaponState state, int param)
//     {
//         EventManger.TriggerEvent(new WeaponStateEvent(state, param));
//     }
// }
//
// public class WeaponStateMachine : StateMachine<WeaponStateEvent>
// {
//     public float Duration;
//     public Action<int> OnAmmoChange;
//
//     public WeaponStateMachine(float duration) : base(null, false)
//     {
//         Duration = duration;
//     }
//
//     public void OnAmmoChanged(Action<int> onAmmoChange)
//     {
//         OnAmmoChange += onAmmoChange;
//     }
// }