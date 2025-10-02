using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform playerRoot; // assigne ta capsule Player; si null => ce GO

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2.0f;
    public float minPitch = -89f;
    public float maxPitch = 89f;

    private Rigidbody _rb;
    private Transform _cameraT;
    private Vector2 _moveInput; // x=right/left, y=fwd/back
    private float _pitch;

    void Awake()
    {
        if (playerRoot == null) playerRoot = transform;
        _rb = playerRoot.GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody manquant sur playerRoot (capsule).");
            return;
        }

        _rb.freezeRotation = true;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.useGravity = true;
    }

    void Start()
    {
        // Récupère automatiquement la Main Camera enfant du Player
        var cam = playerRoot.GetComponentInChildren<Camera>(true);
        if (cam != null)
        {
            _cameraT = cam.transform;
            // initialise pitch à l'angle local actuel
            float x = _cameraT.localEulerAngles.x;
            if (x > 180f) x -= 360f;
            _pitch = Mathf.Clamp(x, minPitch, maxPitch);
            var e = _cameraT.localEulerAngles;
            e.x = _pitch; e.y = 0f; e.z = 0f;
            _cameraT.localEulerAngles = e;
        }
        else
        {
            Debug.LogError("PlayerController: Aucune Camera trouvée en enfant de playerRoot.");
        }
    }

    void OnEnable()
    {
        if (InputController.Instance != null)
        {
            InputController.Instance.OnMove += HandleMove;
            InputController.Instance.OnLook += HandleLook;
        }
    }

    void OnDisable()
    {
        if (InputController.Instance != null)
        {
            InputController.Instance.OnMove -= HandleMove;
            InputController.Instance.OnLook -= HandleLook;
        }
    }

    void HandleMove(Vector2 move)
    {
        _moveInput = move;
    }

    void HandleLook(Vector2 lookDelta)
    {
        if (playerRoot == null) return;

        float yaw = lookDelta.x * mouseSensitivity;
        playerRoot.Rotate(Vector3.up, yaw, Space.Self);

        _pitch -= lookDelta.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        if (_cameraT != null)
        {
            var e = _cameraT.localEulerAngles;
            e.x = _pitch; e.y = 0f; e.z = 0f;
            _cameraT.localEulerAngles = e;
        }
    }

    void FixedUpdate()
    {
        if (_rb == null) return;

        if (_moveInput == Vector2.zero) return;

        Vector3 dir = (playerRoot.right * _moveInput.x) + (playerRoot.forward * _moveInput.y);
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        Vector3 targetPos = _rb.position + dir * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(targetPos);
    }
}
