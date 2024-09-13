using System;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private static TimerManager manager;
    private readonly float duration;
    private float startTime;
    private float? timeElapsedBeforePause;
    private MonoBehaviour owner;
    private bool hasOwner;

    private Action onStart;
    private Action<float> onUpdate;
    private Action<float> onProgress;
    private Action onComplete;
    private Action onDone;

    public float Duration => duration;
    public bool IsCompleted { get; private set; }
    public bool IsPaused => timeElapsedBeforePause.HasValue;
    public bool IsLooped { get; private set; }
    public bool IsCancelled { get; private set; }
    public bool UsesRealTime { get; private set; }
    public bool IsDone => IsCompleted || IsPaused || IsCancelled || isOwnerDestroyed;
    public bool IsRunning => !IsDone;
    public float Progress => GetTimeElapsed() / duration;
    public float TimeRemaining => duration - GetTimeElapsed();
    public float Remaining => duration / GetTimeElapsed();

    private bool isOwnerDestroyed => hasOwner && owner == null;

    private Timer(float duration)
    {
        this.duration = duration;
        startTime = GetWorldTime();
    }

    public static Timer Register(float duration)
    {
        if (manager == null)
            manager = TimerManager.Instance ?? throw new Exception("TimerManager is not found in the scene");

        var timer = new Timer(duration);
        timer.onDone = () => manager.RemoveTimer(timer);
        return timer;
    }

    public Timer OnStart(Action onStart)
    {
        this.onStart = onStart;
        return this;
    }

    public Timer OnUpdate(Action<float> onUpdate)
    {
        this.onUpdate += onUpdate;
        return this;
    }

    public Timer OnProgress(Action<float> onProgress)
    {
        this.onProgress += onProgress;
        return this;
    }

    public Timer OnComplete(Action onComplete)
    {
        this.onComplete = onComplete;
        return this;
    }

    public Timer OnDone(Action onDone)
    {
        this.onDone = onDone;
        return this;
    }

    public Timer Loop(bool isLooped = true)
    {
        IsLooped = isLooped;
        return this;
    }

    public Timer UseRealTime(bool useRealTime = true)
    {
        UsesRealTime = useRealTime;
        return this;
    }

    public Timer AutoDestroyWhenOwnerDestroyed(MonoBehaviour owner)
    {
        if (owner == null) return this;
        this.owner = owner;
        hasOwner = true;
        return this;
    }

    public Timer Start()
    {
        if (IsDone) return null;
        manager.RegisterTimer(this);
        onStart?.Invoke();
        return this;
    }
    
    public Timer AlreadyDone()
    {
        IsCompleted = true;
        return this;
    }

    public void Restart()
    {
        startTime = GetWorldTime();
        IsCompleted = false;
        IsCancelled = false;
        timeElapsedBeforePause = null;
        manager.RegisterTimer(this);
    }

    public void Cancel()
    {
        IsCancelled = true;
    }

    public void Pause()
    {
        if (IsPaused || IsDone) return;
        timeElapsedBeforePause = GetTimeElapsed();
    }

    public void Resume()
    {
        if (!IsPaused) return;
        startTime = GetWorldTime() - timeElapsedBeforePause.Value;
        timeElapsedBeforePause = null;
        manager.RegisterTimer(this);
    }

    private float GetTimeElapsed()
    {
        return IsCompleted ? duration : timeElapsedBeforePause ?? GetWorldTime() - startTime;
    }

    private float GetWorldTime() => UsesRealTime ? Time.realtimeSinceStartup : Time.time;

    public void Update()
    {
        if (IsDone)
        {
            onDone?.Invoke();
            return;
        }

        onUpdate?.Invoke(GetTimeElapsed());
        onProgress?.Invoke(Progress);

        if (!(GetWorldTime() >= startTime + duration)) return;
        onComplete?.Invoke();
        if (IsLooped)
            startTime = GetWorldTime();
        else
        {
            IsCompleted = true;
            onDone?.Invoke();
        }
    }
}

public class TimerManager : Singleton<TimerManager>
{
    private readonly List<Timer> timers = new List<Timer>();

    private void Update()
    {
        UpdateAllTimers();
    }

    public void RegisterTimer(Timer timer)
    {
        timers.Add(timer);
    }

    public void RemoveTimer(Timer timer)
    {
        timers.Remove(timer);
    }

    private void UpdateAllTimers()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            timers[i].Update();
        }
    }

    public void PauseAllTimers()
    {
        foreach (var timer in timers)
        {
            timer.Pause();
        }
    }

    public void ResumeAllTimers()
    {
        foreach (var timer in timers)
        {
            timer.Resume();
        }
    }

    public void CancelAllTimers()
    {
        foreach (var timer in timers)
        {
            timer.Cancel();
        }
    }
}
