// InputController.cs
using System;
using UnityEngine;

public class InputController : BaseController<InputController>
{
    public event Action<Vector2> OnMove; // x = droite/gauche, y = avant/arrière
    public event Action<Vector2> OnLook; // x = yaw, y = pitch
    public event Action<int> OnClick;    // 0 = gauche, 1 = droit, 2 = milieu

    public event Action OnPauseToggle;

    void Start()
    {
        GameController.Instance.OnGameStateChanged += GameStateChanged;
    }

    void Update()
    {
        // Toujours écouter Escape pour toggle pause (même en Paused)
        if (Input.GetKeyDown(KeyCode.Escape))
            OnPauseToggle?.Invoke();

        // Bloquer le reste des inputs si on n'est pas en Running
        if (GameController.Instance.State != GameController.GameState.Running)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(x, y);
        if (move.sqrMagnitude > 1f) move.Normalize();
        OnMove?.Invoke(move);

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        if (mx != 0f || my != 0f) OnLook?.Invoke(new Vector2(mx, my));

        if (Input.GetMouseButtonDown(0))
            OnClick?.Invoke(0);
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
