using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Image cursorImage;
    public Sprite cursorSprite;
    public Sprite activeCursorSprite;

    public GameObject interactablePanel;
    public TMP_Text interactableName;
    public TMP_Text interactableLabelAction;

    void Start()
    {
        if (cursorImage != null)
            cursorImage.sprite = cursorSprite;

        if (interactablePanel != null)
            interactablePanel.SetActive(false);
    }   
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
        if (cursorImage != null)
            cursorImage.sprite = isActive ? activeCursorSprite : cursorSprite;

        if (interactablePanel != null)
            interactablePanel.SetActive(isActive);

        if (interactableName != null && interactableObject != null)
            interactableName.text = interactableObject.customName;
            interactableLabelAction.text = interactableObject.labelAction;
    }
}