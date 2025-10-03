// FadeController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : BaseController<FadeController>
{
    public CanvasGroup fadeCanvasGroup;
    public bool blockInputWhileFading = true;

    Coroutine currentFade;

    void Awake()
    {
        if (fadeCanvasGroup != null)
            SetBlackInstant(false);
    }

    public void Bind(CanvasGroup target)
    {
        fadeCanvasGroup = target;
        SetBlackInstant(false);
    }

    public void FadeToBlack(float duration = 1f)
    {
        if (fadeCanvasGroup == null) return;
        StartFade(1f, duration);
    }

    public void FadeFromBlack(float duration = 1f)
    {
        if (fadeCanvasGroup == null) return;
        StartFade(0f, duration);
    }

    public void SetBlackInstant(bool black)
    {
        if (fadeCanvasGroup == null) return;
        if (currentFade != null) StopCoroutine(currentFade);

        fadeCanvasGroup.alpha = black ? 1f : 0f;

        if (blockInputWhileFading)
        {
            fadeCanvasGroup.blocksRaycasts = black;
            fadeCanvasGroup.interactable = black;
        }
    }

    public void ResetFade()
    {
        SetBlackInstant(false);
    }

    void StartFade(float targetAlpha, float duration)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeRoutine(targetAlpha, Mathf.Max(0.0001f, duration)));
    }

    IEnumerator FadeRoutine(float target, float duration)
    {
        float start = fadeCanvasGroup.alpha;
        float t = 0f;

        if (blockInputWhileFading)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            fadeCanvasGroup.interactable = true;
        }

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // ignore Time.timeScale
            float k = Mathf.Clamp01(t / duration);
            fadeCanvasGroup.alpha = Mathf.Lerp(start, target, k);
            yield return null;
        }

        fadeCanvasGroup.alpha = target;

        if (blockInputWhileFading && Mathf.Approximately(target, 0f))
        {
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }

        currentFade = null;
    }
}
