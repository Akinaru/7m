// GameController.cs
using System;
using UnityEngine;

public class GameController : BaseController<GameController>
{
    [Header("Durée de partie")]
    public float gameDurationSeconds = 7f * 60f; // 7 minutes

    public enum GameState { Idle, Running, Ended }
    public GameState State { get; private set; } = GameState.Idle;

    void OnEnable()
    {
        if (TimerController.Instance != null)
        {
            TimerController.Instance.OnFinished += HandleTimerFinished;
            TimerController.Instance.OnSecondTick += HandleSecondTick;
        }
        this.StartGame();
    }

    void OnDisable()
    {
        if (TimerController.Instance != null)
        {
            TimerController.Instance.OnFinished -= HandleTimerFinished;
            TimerController.Instance.OnSecondTick -= HandleSecondTick;
        }
    }

    public void StartGame()
    {
        if (State == GameState.Running) return;

        State = GameState.Running;
        Debug.Log("[Game] Démarrage de la partie.");
        TimerController.Instance.StartTimer(gameDurationSeconds);
    }

    public void EndGame()
    {
        if (State != GameState.Running) return;

        State = GameState.Ended;
        Debug.Log("[Game] Fin de partie (reçue).");
    }

    public void ResetGame()
    {
        Debug.Log("[Game] Réinitialisation.");
        State = GameState.Idle;
        TimerController.Instance.ResetTimer();
    }

    public void RestartGame()
    {
        ResetGame();
        StartGame();
    }

    void HandleTimerFinished()
    {
        Debug.Log("[Game] Événement de fin de timer reçu.");
        EndGame();
        // Ici tu pourras enchaîner un Restart si tu veux:
        // RestartGame();
    }

    void HandleSecondTick(int elapsedWhole, float elapsed, float remaining)
    {
        // Optionnel: logs côté Game si tu veux voir passer les ticks ici aussi
        // Debug.Log($"[Game] Tick {elapsedWhole}s (restant ~{Mathf.CeilToInt(remaining)}s)");
    }
}
