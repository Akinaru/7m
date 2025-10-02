using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : BaseController<UIController>
{
    public Image cursorImage;
    public Sprite cursorSprite;
    public Sprite activeCursorSprite;

    public GameObject interactablePanel;
    public TMP_Text interactableName;
    public TMP_Text interactableLabelAction;

    public GameObject TitleMenu;

    public delegate void UIEvent();
    public event UIEvent OnButtonStartClicked;

    // Methodes de base du controller

    void Start()
    {
        SetCursorNormal();
        SetPanelActive(false);
    }

    private void OnEnable()
    {
        if (InteractionController.Instance != null)
            InteractionController.Instance.OnLookAtInteractable += OnInteractableLookedAt;
    }

    private void OnDisable()
    {
    }

    // Interactions custom

    private void OnInteractableLookedAt(bool isActive, Interactable interactableObject)
    {
        UpdateCursor(isActive);
        UpdatePanel(isActive, interactableObject);
    }

    private void UpdateCursor(bool isActive)
    {
        if (cursorImage != null)
            cursorImage.sprite = isActive ? activeCursorSprite : cursorSprite;
    }

    private void SetCursorNormal()
    {
        if (cursorImage != null)
            cursorImage.sprite = cursorSprite;
    }

    private void UpdatePanel(bool isActive, Interactable interactableObject)
    {
        SetPanelActive(isActive);

        if (interactableObject != null)
        {
            if (interactableName != null)
                interactableName.text = interactableObject.customName;

            if (interactableLabelAction != null)
                interactableLabelAction.text = interactableObject.labelAction;
        }
    }

    private void SetPanelActive(bool isActive)
    {
        if (interactablePanel != null)
            interactablePanel.SetActive(isActive);
    }

    public void SetTitleMenuActive(bool show)
    {
        if (TitleMenu != null)
            TitleMenu.SetActive(show);
    }

    public void ButtonStartClicked()
    {
        OnButtonStartClicked?.Invoke();
        SetTitleMenuActive(false);
    }
}