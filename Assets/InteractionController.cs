using System;
using UnityEngine;

public class InteractionController : BaseController<InteractionController>
{
    public Camera playerCamera;
    public float rayDistance = 10f;

    public event Action<bool, Interactable> OnLookAtInteractable;

    private Interactable lastObject = null;
    private Interactable interactableObject = null;
    private RaycastHit lastHit; // sauvegarde du hit actuel

    private void OnEnable()
    {
        if (InputController.Instance != null)
            InputController.Instance.OnClick += HandleClick;
    }

    private void OnDisable()
    {
        if (InputController.Instance != null)
            InputController.Instance.OnClick -= HandleClick;
    }

    private void HandleClick(int button)
    {
        if (button == 0) // clic gauche
        {
            if (interactableObject != null)
            {
                switch (interactableObject.customName)
                {
                    case "Cube":
                        // utilise le transform directement via le hit
                        lastHit.collider.transform.position = new Vector3(0, 2, 0);
                        break;

                    default:
                        Debug.Log("Interaction avec : " + interactableObject.customName);
                        break;
                }
            }
        }
    }

    void Update()
    {
        CheckLook();
    }

    void CheckLook()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        bool isLooking = false;

        interactableObject = null; // reset par défaut

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                interactableObject = hit.collider.GetComponent<Interactable>();
                if (interactableObject != null)
                {
                    isLooking = true;
                    lastHit = hit; // sauvegarde du hit actuel
                }
            }
        }

        if (!isLooking)
        {
            lastObject = null;
            OnLookAtInteractable?.Invoke(false, null);
        }
        else if (interactableObject != lastObject)
        {
            OnLookAtInteractable?.Invoke(true, interactableObject);
            lastObject = interactableObject;
        }
    }
}