// GameController.cs
using System.Collections;
using UnityEngine;

public class GameController : BaseController<GameController>
{
    // Variable de jeu
    [Header("Durée de partie")]
    public float gameDurationSeconds = 7f * 60f; // 7 minutes
    [Header("Vitesse du joueur")]
    public float moveSpeed = 5f;
    public const float MIN_MOVE_SPEED = 1f;
    public const float MAX_MOVE_SPEED = 20f;
    [Header("Sensibilité de la souris")]
    public float mouseSensitivity = 1f;
    public const float MIN_MOUSE_SENSITIVITY = 0.1f;
    public const float MAX_MOUSE_SENSITIVITY = 4f;
    public static readonly Vector3 START_POSITION = new Vector3(0f, 1f, 0f);

    // Variable d'état du jeu
    public bool FirstLevierActivated = false;
    public bool SecondLevierActivated = false;
    public bool ThirdLevierActivated = false;

    // Events
    public delegate void GameEvent(GameState state);
    public event GameEvent OnGameStateChanged;

    public event System.Action<bool> OnPauseStateChanged;

    public delegate void GameSettingsEvent(float newValue);
    public event GameSettingsEvent OnMoveSpeedChanged;
    public event GameSettingsEvent OnMouseSensitivityChanged;

    public enum GameState { Idle, Running, Paused, Ended }
    public GameState State { get; private set; } = GameState.Idle;

    GameTimer currentTimer;
    Coroutine timerLoopCo;
    bool isRestarting;

    void OnEnable()
    {
        if (UIController.Instance != null)
        {
            UIController.Instance.OnButtonStartClicked += StartGame;
            UIController.Instance.OnSpeedSliderValueChanged += OnSettingsMoveSpeedChanged;
            UIController.Instance.OnMouseSensitivitySliderValueChanged += OnSettingsMouseSensitivityChanged;
        }

        if (InputController.Instance != null)
            InputController.Instance.OnPauseToggle += TogglePause;


    }

    void OnDisable()
    {
        if (UIController.Instance != null)
            UIController.Instance.OnButtonStartClicked -= StartGame;

        if (InputController.Instance != null)
            InputController.Instance.OnPauseToggle -= TogglePause;

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
        if (State != GameState.Running && State != GameState.Paused) return;

        ChangeState(GameState.Ended);
        UIController.Instance.SetTitleMenuActive(false);
        OnPauseStateChanged?.Invoke(false);
        Time.timeScale = 1f;
    }

    public void ResetGame()
    {
        ChangeState(GameState.Idle);
        OnPauseStateChanged?.Invoke(false);
        Time.timeScale = 1f;
        if (DeplacementController.Instance != null)
            DeplacementController.Instance.ResetPlayerPosition();
        DestroyCurrentTimer();
    }

    public void RestartGame()
    {
        ResetGame();
        StartGame();
    }

    void TogglePause()
    {
        if (State == GameState.Running || State == GameState.Paused)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (State != GameState.Running)
        {
            ChangeState(GameState.Running);

            if (currentTimer != null)
                currentTimer.Resume();

            Time.timeScale = 1f;
            OnPauseStateChanged?.Invoke(false);
        }
        else
        {
            ChangeState(GameState.Paused);

            if (currentTimer != null)
                currentTimer.Pause();

            Time.timeScale = 0f;
            OnPauseStateChanged?.Invoke(true);
        }
    }

    //Methode qui s'execute quand le timer arrive a 0
    void HandleTimerFinished()
    {
        EndGame();
        StartCoroutine(RestartNextFrameRealtime());
    }

    //Methode qui s'execute toutes les secondes pendant la phase de jeu
    void HandleTimerTick(int whole, float elapsed, float remaining)
    {
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

    // Nettoie le timer courant
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

    public void OnSettingsMoveSpeedChanged(float newSpeed)
    {
        moveSpeed = newSpeed;
        OnMoveSpeedChanged?.Invoke(newSpeed);
    }

    public void OnSettingsMouseSensitivityChanged(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        OnMouseSensitivityChanged?.Invoke(newSensitivity);
    }
}
