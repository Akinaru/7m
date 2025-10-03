// InputController.cs
using System;
using UnityEngine;

public class InputController : BaseController<InputController>
{
    public event Action<Vector2> OnMove; // x = droite/gauche, y = avant/arrière
    public event Action<Vector2> OnLook; // x = yaw, y = pitch
    public event Action<int> OnClick;    // 0 = gauche, 1 = droit, 2 = milieu

    public event Action OnPauseToggle;

    float mouseSensitivity = 1f;

    void OnEnable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnGameStateChanged += GameStateChanged;
            GameController.Instance.OnMouseSensitivityChanged += OnSettingsMouseSensitivityChanged;

            mouseSensitivity = GameController.Instance.mouseSensitivity;
        }
    }

    void OnDisable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnGameStateChanged -= GameStateChanged;
            GameController.Instance.OnMouseSensitivityChanged -= OnSettingsMouseSensitivityChanged;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnPauseToggle?.Invoke();

        if (GameController.Instance.State != GameController.GameState.Running)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(x, y);
        if (move.sqrMagnitude > 1f) move.Normalize();
        OnMove?.Invoke(move);

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        if (mx != 0f || my != 0f)
        {
            Vector2 look = new Vector2(mx, my) * mouseSensitivity;
            OnLook?.Invoke(look);
        }

        if (Input.GetMouseButtonDown(0))
            OnClick?.Invoke(0);
    }

    public void OnSettingsMouseSensitivityChanged(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    public void GameStateChanged(GameController.GameState state)
    {
        if (state == GameController.GameState.Running)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
