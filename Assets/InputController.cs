// InputController.cs
using System;
using UnityEngine;

public class InputController : BaseController<InputController>
{
    public event Action<Vector2> OnMove; // x = droite/gauche, y = avant/arrière
    public event Action<Vector2> OnLook; // x = yaw, y = pitch

    public event Action<int> OnClick;    // 0 = gauche, 1 = droit, 2 = milieu


    void Start()
    {
        GameController.Instance.OnGameStateChanged += GameStateChanged;
    }

    void Update()
    {
        if (GameController.Instance.State != GameController.GameState.Running)
            return;
        // Déplacement (ZQSD / flèches)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(x, y);
        if (move.sqrMagnitude > 1f) move.Normalize();
        OnMove?.Invoke(move);

        // Souris
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        if (mx != 0f || my != 0f) OnLook?.Invoke(new Vector2(mx, my));


        // Clic souris
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke(0);
        }
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
