using System;
using UnityEngine;

public class InteractionController : BaseController<InteractionController>
{
    public Camera playerCamera;
    public float rayDistance = 10f;

    // Event qui envoie l’état et l’objet interactif regardé (ou null si rien)
    public event Action<bool, Interactable> OnLookAtInteractable;

    private bool lastState = false;

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
        Interactable interactableObject = null; 

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                isLooking = true;
                interactableObject = hit.collider.GetComponent<Interactable>();
            }
        }

        if (isLooking != lastState)
        {
            OnLookAtInteractable?.Invoke(isLooking, interactableObject);
            lastState = isLooking;
        }
    }
}