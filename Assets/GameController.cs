// GameController.cs
using System.Collections;
using UnityEngine;

public class GameController : BaseController<GameController>
{
    [Header("Durée de partie")]
    public float gameDurationSeconds = 7f * 60f; // 7 minutes

    public delegate void GameEvent(GameState state);
    public event GameEvent OnGameStateChanged;

    public enum GameState { Idle, Running, Ended }
    public GameState State { get; private set; } = GameState.Idle;

    GameTimer currentTimer;
    Coroutine timerLoopCo;
    bool isRestarting;

    void OnEnable()
    {
        if (UIController.Instance != null)
            UIController.Instance.OnButtonStartClicked += StartGame;
    }

    void OnDisable()
    {
        if (UIController.Instance != null)
            UIController.Instance.OnButtonStartClicked -= StartGame;

        DestroyCurrentTimer();
    }

    public void ChangeState(GameState newState)
    {
        if (State == newState) return;
        State = newState;
        OnGameStateChanged?.Invoke(State);
    }

    public void StartGame()
    {
        if (State == GameState.Running) return;

        ChangeState(GameState.Running);
        Time.timeScale = 1f;

        DestroyCurrentTimer();

        currentTimer = new GameTimer(gameDurationSeconds, useUnscaledTime: true);
        currentTimer.OnFinished += HandleTimerFinished;
        currentTimer.OnSecondTick += HandleTimerTick;

        currentTimer.Start();
        timerLoopCo = StartCoroutine(TimerLoop());
    }

    public void EndGame()
    {
        if (State != GameState.Running) return;

        ChangeState(GameState.Ended);
        UIController.Instance.SetTitleMenuActive(false);
    }

    public void ResetGame()
    {
        ChangeState(GameState.Idle);
        DestroyCurrentTimer();
    }

    public void RestartGame()
    {
        ResetGame();
        StartGame();
    }

    void HandleTimerFinished()
    {
        EndGame();
        StartCoroutine(RestartNextFrameRealtime());
    }

    void HandleTimerTick(int whole, float elapsed, float remaining)
    {
        // Place ta logique de tick ici si besoin
    }

    IEnumerator TimerLoop()
    {
        while (currentTimer != null && currentTimer.Update())
            yield return null;

        timerLoopCo = null;
    }

    IEnumerator RestartNextFrameRealtime()
    {
        if (isRestarting) yield break;

        isRestarting = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(0.01f);
        isRestarting = false;

        RestartGame();
    }

    void DestroyCurrentTimer()
    {
        if (timerLoopCo != null)
        {
            StopCoroutine(timerLoopCo);
            timerLoopCo = null;
        }

        if (currentTimer != null)
        {
            currentTimer.OnFinished -= HandleTimerFinished;
            currentTimer.OnSecondTick -= HandleTimerTick;
            currentTimer.Stop();
            currentTimer = null;
        }
    }
}
