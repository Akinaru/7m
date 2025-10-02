using System;
using UnityEngine;

public class InteractionController : BaseController<InteractionController>
{
    public Camera playerCamera;
    public float rayDistance = 10f;

    public event Action<bool, Interactable> OnLookAtInteractable;

    private Interactable lastObject = null;

    void Update()
    {
        CheckLook();
    }

    void CheckLook()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        Interactable interactableObject = null;
        bool isLooking = false;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                interactableObject = hit.collider.GetComponent<Interactable>();
                if (interactableObject != null)
                    isLooking = true;
            }
        }

        if (!isLooking)
            lastObject = null;


        if (isLooking && interactableObject != lastObject)
        {
            OnLookAtInteractable?.Invoke(isLooking, interactableObject);
            lastObject = interactableObject;
        }

        else if (!isLooking)
        {
            OnLookAtInteractable?.Invoke(false, null);
        }
    }
}