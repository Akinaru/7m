using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Image cursorImage;
    public Sprite cursorSprite;
    public Sprite activeCursorSprite;

    public GameObject infoPanel; // panel UI à afficher
    public TMP_Text infoText;

    void Start()
    {
        if (cursorImage != null)
            cursorImage.sprite = cursorSprite;

        if (infoPanel != null)
            infoPanel.SetActive(false);
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

        if (infoPanel != null)
            infoPanel.SetActive(isActive);

        if (infoText != null && interactableObject != null)
            infoText.text = interactableObject.customName;
    }
}