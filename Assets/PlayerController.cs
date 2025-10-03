// PlayerController.cs
using UnityEngine;

public class PlayerController : BaseController<PlayerController>
{
    [Header("Object du joueur")]
    public Transform playerRoot;
    [Header("Camera du joueur (auto assignée)")]
    public Camera playerCamera;
    [Header("Sensibilité de la souris")]
    float mouseSensitivity = 1f;
    float minPitch = -89f;
    float maxPitch = 89f;

    Transform camT;
    Rigidbody rb;
    float yawDeg;
    float pitchDeg;

    void Awake()
    {
        if (!playerRoot) playerRoot = transform;
        if (!playerCamera && playerRoot) playerCamera = playerRoot.GetComponentInChildren<Camera>(true);

        rb = playerRoot.GetComponent<Rigidbody>();
        rb.useGravity = true;

        var cam = playerRoot.GetComponentInChildren<Camera>(true);
        if (cam) camT = cam.transform;

        yawDeg = playerRoot.eulerAngles.y;
        if (camT)
        {
            float x = camT.localEulerAngles.x; if (x > 180f) x -= 360f;
            pitchDeg = Mathf.Clamp(x, minPitch, maxPitch);
            camT.localRotation = Quaternion.Euler(pitchDeg, 0f, 0f);
        }
    }

    void OnEnable()
    {
        this.CheckImport();

        if (InputController.Instance != null)
            InputController.Instance.OnLook += OnLookInput;
    }

    void OnDisable()
    {
        if (InputController.Instance != null)
            InputController.Instance.OnLook -= OnLookInput;
    }

    public void CheckImport()
    {
        if (playerRoot == null)
            Debug.LogError("[PlayerController] playerRoot n'est pas assigné dans l'éditeur.");
    }

    void OnLookInput(Vector2 delta)
    {
        yawDeg += delta.x * mouseSensitivity;
        pitchDeg = Mathf.Clamp(pitchDeg - delta.y * mouseSensitivity, minPitch, maxPitch);

        if (camT) camT.localRotation = Quaternion.Euler(pitchDeg, 0f, 0f);
    }

    void FixedUpdate()
    {
        // Empêche le yaw physique induit par les collisions
        var av = rb.angularVelocity;
        if (av.y != 0f) { av.y = 0f; rb.angularVelocity = av; }

        // Applique notre yaw (fluide et propre)
        rb.MoveRotation(Quaternion.Euler(0f, yawDeg, 0f));
    }
}
