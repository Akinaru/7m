using System;
using UnityEngine;

public class InteractionController : BaseController<InteractionController>
{
    public Camera playerCamera;
    public float rayDistance = 10f;
    public event Action<bool> OnLookAtInteractable;
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

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
                isLooking = true;
        }

        if (isLooking != lastState)
        {
            OnLookAtInteractable?.Invoke(isLooking);
            lastState = isLooking;
        }
    }
}