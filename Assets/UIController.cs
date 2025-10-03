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
    public GameObject WinMenu;
    public Slider SpeedSlider;
    public Slider MouseSensitivitySlider;

    // Ajout: CanvasGroup du panneau noir (plein écran) pour le fade
    public CanvasGroup BlackoutCanvasGroup;

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
        SetWinMenuActive(false);
    }

    private void OnEnable()
    {
        if (InteractionController.Instance != null)
            InteractionController.Instance.OnLookAtInteractable += OnInteractableLookedAt;

        this.CheckImport();

        // Bind du CanvasGroup noir au FadeController
        if (FadeController.Instance != null && BlackoutCanvasGroup != null)
        {
            FadeController.Instance.Bind(BlackoutCanvasGroup);
            FadeController.Instance.SetBlackInstant(false);
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.OnPauseStateChanged += HandlePauseStateChanged;
            GameController.Instance.OnGameStateChanged += HandleGameStateChanged;

            if (SpeedSlider != null)
            {
                SpeedSlider.minValue = GameController.MIN_MOVE_SPEED;
                SpeedSlider.maxValue = GameController.MAX_MOVE_SPEED;
                SpeedSlider.value = GameController.Instance.moveSpeed;
            }
            if (MouseSensitivitySlider != null)
            {
                MouseSensitivitySlider.minValue = GameController.MIN_MOUSE_SENSITIVITY;
                MouseSensitivitySlider.maxValue = GameController.MAX_MOUSE_SENSITIVITY;
                MouseSensitivitySlider.value = GameController.Instance.mouseSensitivity;
            }
        }
    }

    private void OnDisable()
    {
        if (InteractionController.Instance != null)
            InteractionController.Instance.OnLookAtInteractable -= OnInteractableLookedAt;

        if (GameController.Instance != null)
            GameController.Instance.OnPauseStateChanged -= HandlePauseStateChanged;
    }

    public void CheckImport()
    {
        if (cursorImage == null)
            Debug.LogError("[UIController] cursorImage n'est pas assigné dans l'inspecteur.");
        if (cursorSprite == null)
            Debug.LogError("[UIController] cursorSprite n'est pas assigné dans l'inspecteur.");
        if (activeCursorSprite == null)
            Debug.LogError("[UIController] activeCursorSprite n'est pas assigné dans l'inspecteur.");
        if (interactablePanel == null)
            Debug.LogError("[UIController] interactablePanel n'est pas assigné dans l'inspecteur.");
        if (interactableName == null)
            Debug.LogError("[UIController] interactableName n'est pas assigné dans l'inspecteur.");
        if (interactableLabelAction == null)
            Debug.LogError("[UIController] interactableLabelAction n'est pas assigné dans l'inspecteur.");
        if (TitleMenu == null)
            Debug.LogError("[UIController] TitleMenu n'est pas assigné dans l'inspecteur.");
        if (PauseMenu == null)
            Debug.LogError("[UIController] PauseMenu n'est pas assigné dans l'inspecteur.");
        if (SpeedSlider == null)
            Debug.LogError("[UIController] SpeedSlider n'est pas assigné dans l'inspecteur.");
        if (MouseSensitivitySlider == null)
            Debug.LogError("[UIController] MouseSensitivitySlider n'est pas assigné dans l'inspecteur.");
        if (WinMenu == null)
            Debug.LogError("[UIController] WinMenu n'est pas assigné dans l'inspecteur.");
        if (BlackoutCanvasGroup == null)
            Debug.LogError("[UIController] BlackoutCanvasGroup (fade) n'est pas assigné dans l'inspecteur.");
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
                interactableName.text = interactableObject.DisplayName;

            if (interactableLabelAction != null)
                interactableLabelAction.text = interactableObject.ActionLabel;
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

    public void SetPauseMenuActive(bool show)
    {
        if (PauseMenu != null)
            PauseMenu.SetActive(show);
    }

    public void SetWinMenuActive(bool show)
    {
        if (WinMenu != null)
            WinMenu.SetActive(show);
    }

    private void HandlePauseStateChanged(bool isPaused)
    {
        SetPauseMenuActive(isPaused);
    }

    private void HandleGameStateChanged(GameController.GameState newState)
    {
        if (newState == GameController.GameState.Win)
        {
            SetWinMenuActive(true);
        }
    }

    public void ButtonStartClicked()
    {
        OnButtonStartClicked?.Invoke();
        SetTitleMenuActive(false);
    }

    public void ButtonResumeClicked()
    {
        if (GameController.Instance != null)
            GameController.Instance.PauseGame();
    }

    public void OnSpeedValueChangeInMenu(float speed)
    {
        OnSpeedSliderValueChanged?.Invoke(speed);
    }

    public void OnMouseSensitivityValueChangeInMenu(float sensitivity)
    {
        OnMouseSensitivitySliderValueChanged?.Invoke(sensitivity);
    }

    // ---- Contrôles de fade via UIController ----

    public void FadeToBlack(float duration = 1f)
    {
        if (FadeController.Instance != null)
            FadeController.Instance.FadeToBlack(duration);
    }

    public void FadeFromBlack(float duration = 1f)
    {
        if (FadeController.Instance != null)
            FadeController.Instance.FadeFromBlack(duration);
    }

    public void SetBlackoutInstant(bool black)
    {
        if (FadeController.Instance != null)
            FadeController.Instance.SetBlackInstant(black);
    }

    public void ResetBlackout()
    {
        if (FadeController.Instance != null)
            FadeController.Instance.ResetFade();
    }
}
