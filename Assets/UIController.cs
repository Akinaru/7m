// UIController.cs
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
    public GameObject PauseMenu;
    public Slider SpeedSlider;
    public Slider MouseSensitivitySlider;

    public delegate void UIEvent();
    public delegate void UIEventValueChange(float value);

    public event UIEvent OnButtonStartClicked;
    public event UIEventValueChange OnSpeedSliderValueChanged;
    public event UIEventValueChange OnMouseSensitivitySliderValueChanged;

    void Start()
    {
        SetCursorNormal();
        SetPanelActive(false);
        SetPauseMenuActive(false);
    }

    private void OnEnable()
    {
        if (InteractionController.Instance != null)
            InteractionController.Instance.OnLookAtInteractable += OnInteractableLookedAt;

        if (GameController.Instance != null)
        {
            GameController.Instance.OnPauseStateChanged += HandlePauseStateChanged;
            if (SpeedSlider != null)
            {
                SpeedSlider.minValue = GameController.MIN_MOVE_SPEED;
                SpeedSlider.maxValue = GameController.MAX_MOVE_SPEED;
                SpeedSlider.value = GameController.Instance.moveSpeed;
            }
            else
            {
                Debug.LogError("[UIController] SpeedSlider n'est pas assigné dans l'inspecteur.");
            }
            if (MouseSensitivitySlider != null)
            {
                MouseSensitivitySlider.minValue = GameController.MIN_MOUSE_SENSITIVITY;
                MouseSensitivitySlider.maxValue = GameController.MAX_MOUSE_SENSITIVITY;
                MouseSensitivitySlider.value = GameController.Instance.mouseSensitivity;
            }
            else
            {
                Debug.LogError("[UIController] MouseSensitivitySlider n'est pas assigné dans l'inspecteur.");
            }
        }
        else
        {
            Debug.LogError("[UIController] GameController n'a pas d'instance.");
        }
    }

    private void OnDisable()
    {
        if (InteractionController.Instance != null)
            InteractionController.Instance.OnLookAtInteractable -= OnInteractableLookedAt;

        if (GameController.Instance != null)
            GameController.Instance.OnPauseStateChanged -= HandlePauseStateChanged;
    }

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

    // Changer l'état d'afichage du panneau d'interaction
    private void SetPanelActive(bool isActive)
    {
        if (interactablePanel != null)
            interactablePanel.SetActive(isActive);
    }

    // Changer l'état d'afichage du Menu Titre
    public void SetTitleMenuActive(bool show)
    {
        if (TitleMenu != null)
            TitleMenu.SetActive(show);
    }

    // Changer l'état d'afichage du Menu Pause
    public void SetPauseMenuActive(bool show)
    {
        if (PauseMenu != null)
            PauseMenu.SetActive(show);
    }

    private void HandlePauseStateChanged(bool isPaused)
    {
        SetPauseMenuActive(isPaused);
    }


    // Methode lors du clique sur le bouton de start
    public void ButtonStartClicked()
    {
        OnButtonStartClicked?.Invoke();
        SetTitleMenuActive(false);
    }


    // Methode lors du clique sur le bouton de resume
    public void ButtonResumeClicked()
    {
        if (GameController.Instance != null)
            GameController.Instance.PauseGame();
    }

    // Methode lors du changement de la valeur du slider de vitesse 
    public void OnSpeedValueChangeInMenu(float speed)
    {
        OnSpeedSliderValueChanged?.Invoke(speed);
    }

    // Methode lors du changement de la valeur du slider de sensibilité de la souris 
    public void OnMouseSensitivityValueChangeInMenu(float sensitivity)
    {
        OnMouseSensitivitySliderValueChanged?.Invoke(sensitivity);
    }
}
