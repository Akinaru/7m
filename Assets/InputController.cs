using System;
using UnityEngine;

public class InputController : BaseController<InputController>
{
    public event Action<Vector2> OnMove; // x = droite/gauche, y = avant/arrière
    public event Action<Vector2> OnLook; // x = yaw, y = pitch (delta souris)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ZQSD (AZERTY) + flèches + fallback axes
        int right = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0)
                  - (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
        int fwd = (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0)
                  - (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1 : 0);

        // Fallback axes (si jamais les touches au-dessus ne captent pas)
        if (right == 0 && fwd == 0)
        {
            float axH = Input.GetAxisRaw("Horizontal"); // -1..1
            float axV = Input.GetAxisRaw("Vertical");   // -1..1
            if (Mathf.Abs(axH) > 0.01f || Mathf.Abs(axV) > 0.01f)
            {
                right = axH > 0.01f ? 1 : (axH < -0.01f ? -1 : 0);
                fwd = axV > 0.01f ? 1 : (axV < -0.01f ? -1 : 0);
            }
        }

        OnMove?.Invoke(new Vector2(right, fwd));

        // Souris (delta)
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        if (mx != 0f || my != 0f)
        {
            OnLook?.Invoke(new Vector2(mx, my));
        }
    }

    public void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
