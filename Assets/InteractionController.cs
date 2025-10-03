// InteractionController.cs
using System;
using UnityEngine;

public class InteractionController : BaseController<InteractionController>
{
    float rayDistance = 10f;

    public event Action<bool, Interactable> OnLookAtInteractable;

    private Interactable lastObject = null;
    private Interactable interactableObject = null;
    private RaycastHit lastHit;

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
        if (button != 0) return;
        if (interactableObject == null) return;

        // Alimente le contexte avant toute action
        if (ActionController.Instance != null)
        {
            ActionController.Instance.SetContext(new ActionController.InteractionContext
            {
                Hit = lastHit,
                Interactable = interactableObject
            });
        }

        if (interactableObject.OnInteract != null)
        {
            interactableObject.OnInteract.Invoke(interactableObject);
            return;
        }

        var id = interactableObject.Id;
        if (!string.IsNullOrEmpty(id))
        {
            switch (id)
            {
                case "cube":
                    lastHit.collider.transform.position = new Vector3(0, 2, 0);
                    break;
                default:
                    Debug.Log("Interaction sans event avec id: " + id);
                    break;
            }
            return;
        }

        var name = interactableObject.DisplayName ?? "(no name)";
        switch (name)
        {
            case "Cube":
                lastHit.collider.transform.position = new Vector3(0, 2, 0);
                break;
            default:
                Debug.Log("Interaction avec : " + name);
                break;
        }
    }

    void Update()
    {
        CheckLook();
    }

    void CheckLook()
    {
        if (PlayerController.Instance.playerCamera == null) return;

        Ray ray = PlayerController.Instance.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        bool isLooking = false;
        interactableObject = null;

        if (Physics.Raycast(ray, out var hit, rayDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                var it = hit.collider.GetComponent<Interactable>();
                if (it != null)
                {
                    interactableObject = it;
                    isLooking = true;
                    lastHit = hit;
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
