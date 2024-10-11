using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IState
{
    void OnEnter();
    void OnUpdate();
    void OnFixedUpdate();
    void OnExit();
}

public interface IPredicate
{
    bool Evaluate();
}

public class FuncPredicate : IPredicate
{
    private readonly Func<bool> func;
    public FuncPredicate(Func<bool> func) { this.func = func; }
    public bool Evaluate() => func.Invoke();
}

public interface ITransition
{
    IState To { get; }
    IPredicate Condition { get; }
}

public class Transition : ITransition
{
    public IState To { get; }
    public IPredicate Condition { get; }

    public Transition(IState to, IPredicate condition)
    {
        To = to;
        Condition = condition;
    }
}

public class StateMachine
{
    private StateNode defaultState;
    private StateNode current;
    private Dictionary<Type, StateNode> nodes = new();
    private HashSet<ITransition> anyTransitions = new();
    public IState CurrentState => current.State;

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
            ChangeState(transition.To);
        current.State?.OnUpdate();
    }

    public void FixedUpdate() { current.State?.OnFixedUpdate(); }

    public void SetState(IState state)
    {
        defaultState ??= GetOrAddStateNode(state);
        current = GetOrAddStateNode(state);
        current.State?.OnEnter();
    }

    private void ChangeState(IState state)
    {
        if (state == current.State) return;
        current?.State?.OnExit();
        current = GetOrAddStateNode(state);
        current.State?.OnEnter();
    }

    public void At(IState from, IState to, IPredicate condition)
    {
        var fromNode = GetOrAddStateNode(from);
        fromNode.AddTransition(to, condition);
    }

    public void Any(IState to, IPredicate condition) { anyTransitions.Add(new Transition(to, condition)); }

    public void Any(IState to,ref Action action)
    {
        var toNode = GetOrAddStateNode(to);
        action += () => ChangeState(toNode.State);
    }

    private StateNode GetOrAddStateNode(IState state)
    {
        var type = state.GetType();
        if (nodes.TryGetValue(type, out var node)) return node;
        node = new StateNode(state);
        nodes[type] = node;
        return node;
    }

    private ITransition GetTransition()
    {
        foreach (var transition in anyTransitions.Where(transition => transition.Condition.Evaluate()))
            return transition;
        return current.Transitions.FirstOrDefault(transition => transition.Condition.Evaluate());
    }
    
    public void Reset()
    {
        current = defaultState;
        current.State?.OnEnter();
    }

    private class StateNode
    {
        public IState State { get; }
        public HashSet<ITransition> Transitions { get; } = new();
        public StateNode(IState state) { State = state; }

        public void AddTransition(IState to, IPredicate condition) { Transitions.Add(new Transition(to, condition)); }
    }
}

// class IdleState : IState
// {
//     public void OnEnter()
//     {
//         Debug.Log("IdleState OnEnter");
//     }
//     public void Update() { }
//     public void FixedUpdate() { }
//     public void OnExit() { }
// }
//
// class MoveState : IState
// {
//     public void OnEnter()
//     {
//         Debug.Log("MoveState OnEnter");
//     }
//     public void Update() { }
//     public void FixedUpdate() { }
//     public void OnExit() { }
// }
//
// class StateComp
// {
//     StateMachine stateMachine = new ();
//
//     public StateComp() {
//         var idleState = new IdleState();
//         var moveState = new MoveState();
//         
//         At(idleState, moveState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Space)));
//         At(moveState, idleState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.Space)));
//         Any(idleState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.I)));
//         Any(moveState, new FuncPredicate(() => Input.GetKeyDown(KeyCode.M)));
//         
//         stateMachine.SetState(idleState);
//     }
//
//     public void Update()
//     {
//         stateMachine.Update();
//     }
//     
//     public void FixedUpdate()
//     {
//         stateMachine.FixedUpdate();
//     }
//
//     private void At(IState from, IState to, IPredicate condition) =>
//         stateMachine.AddTransition(from, to, condition);
//     private void Any(IState to, IPredicate condition) =>
//         stateMachine.AddAnyTransition(to, condition);
// }