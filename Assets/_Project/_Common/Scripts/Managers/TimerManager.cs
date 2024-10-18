using System;
using System.Collections.Generic;
using Photon.Pun;
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
    private Action<float> onRemaining;
    private Action onComplete;
    private Action onDone;

    public bool IsRegistered { get; private set; }
    public float Duration => duration;
    public bool IsCompleted { get; private set; }
    public bool IsPaused => timeElapsedBeforePause.HasValue;
    public bool IsLooped { get; private set; }
    public bool IsCancelled { get; private set; }
    public bool UsesRealTime { get; private set; }
    public bool IsDone => IsCompleted || IsCancelled || isOwnerDisappear;
    public bool IsRunning => !IsDone;
    public float Progress => GetTimeElapsed() / duration;
    public float TimeRemaining => duration - GetTimeElapsed();
    public float Remaining => duration / GetTimeElapsed();

    private bool isOwnerDisappear => hasOwner && (owner == null || !owner.gameObject.activeSelf);

    private Timer(float duration) { this.duration = duration; }

    public static Timer Register(float duration)
    {
        if (manager == null)
            manager = TimerManager.Instance ?? new GameObject("TimerManager").AddComponent<TimerManager>();

        var timer = new Timer(duration);
        timer.OnStart(() => timer.IsRegistered = true);
        timer.OnDone(() =>
        {
            timer.IsRegistered = false;
            manager.RemoveTimer(timer);
        });
        return timer;
    }

    public Timer OnStart(Action onStart)
    {
        this.onStart += onStart;
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

    public Timer OnRemaining(Action<float> onRemaining)
    {
        this.onRemaining += onRemaining;
        return this;
    }

    public Timer OnComplete(Action onComplete)
    {
        this.onComplete += onComplete;
        return this;
    }

    public Timer OnDone(Action onDone)
    {
        this.onDone += onDone;
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

    public Timer AutoDestroyWhenOwnerDisappear(MonoBehaviour owner)
    {
        if (owner == null) return this;
        this.owner = owner;
        hasOwner = true;
        return this;
    }

    /// <summary>
    /// Your timer will start counting down from the moment you call this method. This method just runs only once no matter how many times you call it.
    /// </summary>
    /// <returns></returns>
    public Timer Start()
    {
        if (IsRegistered) return this;
        if (IsDone) return null;
        startTime = GetWorldTime();
        manager.RegisterTimer(this);
        onStart?.Invoke();
        return this;
    }

    /// <summary>
    /// Your timer will ready to be completed. Usefully when you want to skip the timer.
    /// </summary>
    /// <returns></returns>
    public Timer AlreadyDone()
    {
        IsCompleted = true;
        return this;
    }

    /// <summary>
    /// Your timer will start counting down from the moment you call this method. This method can be called multiple times and it will reset the timer.
    /// </summary>
    /// <returns></returns>
    public Timer Reset()
    {
        if(IsRegistered) manager.RemoveTimer(this);
        startTime = GetWorldTime();
        IsCompleted = false;
        IsCancelled = false;
        timeElapsedBeforePause = null;
        return Start();
    }

    public void Cancel() { IsCancelled = true; }

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
    }

    private float GetTimeElapsed()
    {
        return IsCompleted ? duration : timeElapsedBeforePause ?? GetWorldTime() - startTime;
    }

    private float GetWorldTime() => (float)(UsesRealTime ? Time.realtimeSinceStartup : PhotonNetwork.Time);

    public void Update()
    {
        if (IsDone)
        {
            onDone?.Invoke();
            return;
        }

        if (IsPaused) return;

        onUpdate?.Invoke(GetTimeElapsed());
        onProgress?.Invoke(Progress);
        onRemaining?.Invoke(TimeRemaining);

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

public class TimerManager : PersistentSingleton<TimerManager>, IEventListener<GameEvent>
{
    private readonly List<Timer> timers = new List<Timer>();
    public int sizeOfTimers; //TODO: Delete

    private void Update()
    {
        UpdateAllTimers();
        sizeOfTimers = timers.Count;
    }

    public void RegisterTimer(Timer timer) { timers.Add(timer); }

    public void RemoveTimer(Timer timer) { timers.Remove(timer); }

    private void UpdateAllTimers()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            timers[i].Update();
        }
    }

    private void PauseAllTimers()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            timers[i].Pause();
        }
    }

    private void ResumeAllTimers()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            timers[i].Resume();
        }
    }

    private void CancelAllTimers()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            timers[i].Cancel();
        }

        timers.Clear();
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameMainMenu:
                // Time.timeScale = 1;
                break;
            case GameEventType.GamePreStart:
                // CancelAllTimers();
                // Time.timeScale = 0;
                break;
            case GameEventType.GamePause:
                // PauseAllTimers();
                // Time.timeScale = 0;
                break;
            case GameEventType.GameStart:
                // ResumeAllTimers();
                // Time.timeScale = 1;
                break;
            case GameEventType.GameOver:
                // CancelAllTimers();
                break;
        }
    }

    private void OnEnable() { this.StartListening(); }

    private void OnDisable() { this.StopListening(); }
}