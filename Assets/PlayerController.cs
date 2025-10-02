using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Links")]
    public Transform playerRoot;                   // Capsule réelle (Rigidbody + CapsuleCollider)

    [Header("Movement")]
    public float moveSpeed = 5f;                   // m/s
    public LayerMask environmentMask = ~0;         // Layers des murs/sols
    public float slideProbeDistance = 0.3f;        // Portée du check devant (m)

    [Tooltip("Prend en compte la friction du PhysicMaterial du collider touché.")]
    public bool usePhysicsMaterialFriction = true;

    [Range(0f, 1f), Tooltip("Facteur max de ralentissement dû au frottement lors du slide le long d'un mur.")]
    public float maxWallFriction = 0.6f;           // 0 = pas de friction, 1 = friction très forte

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float minPitch = -89f;
    public float maxPitch = 89f;

    // Cached
    Rigidbody rb;
    CapsuleCollider capsule;
    Transform camT;

    // Inputs/State
    Vector2 moveInput;                             // x = right/left, y = forward/back
    float yawDegrees;                              // rotation Y (deg)
    float pitchDegrees;                            // rotation X caméra (deg)

    void Awake()
    {
        if (!playerRoot) playerRoot = transform;

        rb = playerRoot.GetComponent<Rigidbody>();
        capsule = playerRoot.GetComponent<CapsuleCollider>();

        if (!rb || !capsule)
        {
            Debug.LogError("PlayerController: Rigidbody ou CapsuleCollider manquant sur playerRoot.");
            enabled = false;
            return;
        }

        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.drag = 1f;
        rb.angularDrag = 6f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                        | RigidbodyConstraints.FreezeRotationY
                        | RigidbodyConstraints.FreezeRotationZ;
        rb.maxAngularVelocity = 0f;
    }

    void Start()
    {
        var cam = playerRoot.GetComponentInChildren<Camera>(true);
        if (cam)
        {
            camT = cam.transform;
            float x = camT.localEulerAngles.x; if (x > 180f) x -= 360f;
            pitchDegrees = Mathf.Clamp(x, minPitch, maxPitch);
            var e = camT.localEulerAngles; e.x = pitchDegrees; e.y = 0f; e.z = 0f; camT.localEulerAngles = e;
        }
        else
        {
            Debug.LogError("PlayerController: Aucune Camera trouvée sous playerRoot.");
        }

        yawDegrees = playerRoot.eulerAngles.y;
    }

    void OnEnable()
    {
        if (InputController.Instance != null)
        {
            InputController.Instance.OnMove += OnMoveInput;
            InputController.Instance.OnLook += OnLookInput;
        }
    }

    void OnDisable()
    {
        if (InputController.Instance != null)
        {
            InputController.Instance.OnMove -= OnMoveInput;
            InputController.Instance.OnLook -= OnLookInput;
        }
    }

    void OnMoveInput(Vector2 move) => moveInput = move;

    void OnLookInput(Vector2 mouseDelta)
    {
        yawDegrees += mouseDelta.x * mouseSensitivity;                                      // yaw = corps
        pitchDegrees = Mathf.Clamp(pitchDegrees - mouseDelta.y * mouseSensitivity, minPitch, maxPitch); // pitch = caméra

        if (camT)
        {
            var e = camT.localEulerAngles;
            e.x = pitchDegrees; e.y = 0f; e.z = 0f;
            camT.localEulerAngles = e;
        }
    }

    void FixedUpdate()
    {
        if (!rb) return;

        // Applique la rotation voulue (physique ne tourne plus le corps)
        Quaternion targetRot = Quaternion.Euler(0f, yawDegrees, 0f);
        rb.MoveRotation(targetRot);

        // Direction selon orientation cible
        Vector3 right = targetRot * Vector3.right;
        Vector3 forward = targetRot * Vector3.forward;
        Vector3 wishDir = moveInput == Vector2.zero ? Vector3.zero : (right * moveInput.x + forward * moveInput.y);
        if (wishDir.sqrMagnitude > 1f) wishDir.Normalize();

        // Slide + friction le long des murs
        float finalSpeed = moveSpeed;
        if (wishDir != Vector3.zero && HitWallAhead(wishDir, out RaycastHit wallHit))
        {
            // Ignore les surfaces trop horizontales (sol)
            float wallUpDot = Vector3.Dot(wallHit.normal, Vector3.up);
            if (wallUpDot < 0.8f)
            {
                // Projette sur le plan du mur → slide
                Vector3 slideDir = Vector3.ProjectOnPlane(wishDir, wallHit.normal);
                if (slideDir.sqrMagnitude > 0.0001f)
                {
                    slideDir.Normalize();
                    wishDir = slideDir;

                    // Applique une friction de surface (ralentissement tangentiel)
                    float friction = 0.0f;
                    if (usePhysicsMaterialFriction)
                    {
                        var pm = wallHit.collider.sharedMaterial != null
                            ? wallHit.collider.sharedMaterial
                            : wallHit.collider.material;
                        if (pm != null)
                            friction = Mathf.Max(pm.dynamicFriction, pm.staticFriction); // 0..1 (en général)
                    }

                    friction = Mathf.Clamp01(friction);
                    float slowdown = Mathf.Clamp01(maxWallFriction * friction); // combien on ralentit
                    finalSpeed = moveSpeed * (1f - slowdown);
                }
                else
                {
                    // Face au mur → pas de mouvement avant
                    wishDir = Vector3.zero;
                }
            }
        }

        // Applique la vitesse (préserve gravité)
        float vy = rb.velocity.y;
        Vector3 hv = wishDir * finalSpeed;
        rb.velocity = new Vector3(hv.x, vy, hv.z);

        // Pas de rotation parasite
        rb.angularVelocity = Vector3.zero;
    }

    bool HitWallAhead(Vector3 moveDir, out RaycastHit hit)
    {
        GetCapsuleWorldPoints(out Vector3 p1, out Vector3 p2, out float radius);
        return Physics.CapsuleCast(
            p1, p2, radius * 0.98f,
            moveDir.normalized,
            out hit,
            slideProbeDistance,
            environmentMask,
            QueryTriggerInteraction.Ignore
        );
    }

    void GetCapsuleWorldPoints(out Vector3 p1, out Vector3 p2, out float radius)
    {
        Vector3 lossy = playerRoot.lossyScale;
        float scaleY = Mathf.Abs(lossy.y);
        float scaleXZ = Mathf.Max(Mathf.Abs(lossy.x), Mathf.Abs(lossy.z));

        radius = capsule.radius * scaleXZ;
        float half = Mathf.Max(capsule.height * 0.5f * scaleY - radius, 0f);

        Vector3 centerWorld = playerRoot.TransformPoint(capsule.center);
        Vector3 upWorld = playerRoot.up;

        p1 = centerWorld + upWorld * half; // top
        p2 = centerWorld - upWorld * half; // bottom
    }
}
