// DeplacementController.cs
using UnityEngine;

public class DeplacementController : BaseController<DeplacementController>
{
    public float moveSpeed = 5f;

    Rigidbody rb;
    Transform playerRoot;
    Vector2 moveInput;

    void Awake()
    {
        var pc = PlayerController.Instance;
        playerRoot = pc && pc.playerRoot ? pc.playerRoot : transform;
        rb = playerRoot.GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (InputController.Instance != null)
            InputController.Instance.OnMove += OnMoveInput;
    }

    void OnDisable()
    {
        if (InputController.Instance != null)
            InputController.Instance.OnMove -= OnMoveInput;
    }

    void OnMoveInput(Vector2 v) => moveInput = v;

    void FixedUpdate()
    {
        if (GameController.Instance.State != GameController.GameState.Running)
            return;
        Vector3 dir = playerRoot.right * moveInput.x + playerRoot.forward * moveInput.y;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        Vector3 hv = dir * moveSpeed;
        rb.velocity = new Vector3(hv.x, rb.velocity.y, hv.z); // conserve la gravité
    }
}
