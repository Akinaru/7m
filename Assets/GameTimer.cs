// GameTimer.cs
using System;
using UnityEngine;

public class GameTimer
{
    public bool IsRunning { get; private set; }
    public float DurationSeconds { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public float RemainingSeconds => Mathf.Max(0f, DurationSeconds - ElapsedSeconds);

    public event Action<int, float, float> OnSecondTick; // (elapsedWhole, elapsed, remaining)
    public event Action OnFinished;

    readonly bool useUnscaledTime;
    int lastWhole = -1;

    public GameTimer(float durationSeconds, bool useUnscaledTime = true)
    {
        DurationSeconds = Mathf.Max(0.0001f, durationSeconds);
        this.useUnscaledTime = useUnscaledTime;
    }

    public void Start()
    {
        IsRunning = true;
        ElapsedSeconds = 0f;
        lastWhole = -1;
    }

    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
    }

    // Appelé une fois par frame depuis GameController
    // Retourne true tant que le timer continue, false quand terminé.
    public bool Update()
    {
        if (!IsRunning) return false;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        ElapsedSeconds += dt;

        int whole = Mathf.FloorToInt(ElapsedSeconds);
        if (whole != lastWhole)
        {
            lastWhole = whole;
            float remaining = RemainingSeconds;
            OnSecondTick?.Invoke(whole, ElapsedSeconds, remaining);
        }

        if (ElapsedSeconds >= DurationSeconds)
        {
            IsRunning = false;
            OnFinished?.Invoke();
            return false;
        }

        return true;
    }
}
