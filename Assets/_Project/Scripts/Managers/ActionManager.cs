using System;
using System.Collections.Generic;
using UnityEngine;

public class Event<T>
{
    private Action<T> action;
    
    public void Invoke(T e)
    {
        action?.Invoke(e);
    }
    
    public void AddListener(Action<T> listener)
    {
        action += listener;
    }
    
    public void RemoveListener(Action<T> listener)
    {
        action -= listener;
    }
}
