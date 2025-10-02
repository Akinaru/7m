using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image cursorImage;
    public Sprite cursorSprite;
    public Sprite activeCursorSprite;

    private void OnEnable()
    {
        if (InteractionController.Instance != null)
            InteractionController.Instance.OnLookAtInteractable += UpdateCursor;
    }

    private void OnDisable()
    {
        if (InteractionController.Instance != null)
            InteractionController.Instance.OnLookAtInteractable -= UpdateCursor;
    }

    private void UpdateCursor(bool isActive, Interactable interactableObject)
    {
        if (interactableObject != null)
            {
                Debug.Log("Looking at: " + interactableObject.customName);
            }

        if (cursorImage == null) return;
        cursorImage.sprite = isActive ? activeCursorSprite : cursorSprite;
    }
}