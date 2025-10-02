// TimerController.cs
using System;
using System.Collections;
using UnityEngine;

public class TimerController : BaseController<TimerController>
{
    public bool IsRunning { get; private set; }
    public float DurationSeconds { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public float RemainingSeconds => Mathf.Max(0f, DurationSeconds - ElapsedSeconds);

    public event Action<int, float, float> OnSecondTick; // (elapsedWhole, elapsed, remaining)
    public event Action OnFinished;

    Coroutine co;

    public void StartTimer(float durationSeconds)
    {
        if (durationSeconds <= 0f) durationSeconds = 0.0001f;

        StopTimer();
        DurationSeconds = durationSeconds;
        ElapsedSeconds = 0f;
        IsRunning = true;
        co = StartCoroutine(TimerLoop());
    }

    public void StopTimer()
    {
        if (co != null) { StopCoroutine(co); co = null; }
        IsRunning = false;
    }

    public void ResetTimer()
    {
        StopTimer();
        DurationSeconds = 0f;
        ElapsedSeconds = 0f;
    }

    IEnumerator TimerLoop()
    {
        int lastWhole = -1;

        while (ElapsedSeconds < DurationSeconds)
        {
            ElapsedSeconds += Time.deltaTime;

            int whole = Mathf.FloorToInt(ElapsedSeconds);
            if (whole != lastWhole)
            {
                lastWhole = whole;
                float remaining = Mathf.Max(0f, DurationSeconds - ElapsedSeconds);
                OnSecondTick?.Invoke(whole, ElapsedSeconds, remaining);
                Debug.Log($"[Timer] écoulé: {whole}s | restant: {Mathf.CeilToInt(remaining)}s");
                TriggerStaticEvents(whole, ElapsedSeconds, remaining);
            }

            yield return null;
        }

        IsRunning = false;
        co = null;
        OnFinished?.Invoke();
    }

    void TriggerStaticEvents(int elapsedWhole, float elapsed, float remaining)
    {
        //Si on est à 10 secondes de jeu on trigger l'event Avion
        if (Mathf.CeilToInt(remaining) == 10)
        {
            Debug.Log("Trigger evenement avion");
        }

        //Si on est à 15 secondes de jeu on trigger l'event Chauffeur
        if (Mathf.CeilToInt(remaining) == 15)
        {
            Debug.Log("Trigger evenement chauffeur");
        }
    }
}
