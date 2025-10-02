using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

public class UIController : MonoBehaviour
{
     public Image cursorImage;
    public Sprite cursorSprite;
    public Sprite activeCursorSprite;

    private bool isActive = false;
    public Camera playerCamera;      
    public float rayDistance = 10f;  

    void Update()
    {
        CheckObjectInCenter();
    }

    void CheckObjectInCenter()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                if (!isActive) ToggleCursor(true);
            }
            else
            {
                if (isActive) ToggleCursor(false);
            }
        }
        else
        {
            if (isActive) ToggleCursor(false);
        }
    }

    void ToggleCursor(bool active)
    {
        isActive = active;
        cursorImage.sprite = isActive ? activeCursorSprite : cursorSprite;
    }
}