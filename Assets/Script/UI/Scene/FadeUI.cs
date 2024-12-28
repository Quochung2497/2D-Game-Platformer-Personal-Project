using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    public enum FadeDirection
    {
        In,
        Out
    }

    public virtual void FadeUIIn(CanvasGroup canvasGroup,float _seconds)
    {
        StartCoroutine(Fade(canvasGroup, FadeDirection.In, _seconds));
    }

    public virtual void FadeUIOut(CanvasGroup canvasGroup,float _seconds)
    {
        StartCoroutine(Fade(canvasGroup, FadeDirection.Out, _seconds));
    }
    private IEnumerator Fade(CanvasGroup canvasGroup, FadeDirection fadeDirection, float fadeTime)
    {
        float startAlpha = fadeDirection == FadeDirection.In ? 0f : 1f;
        float endAlpha = fadeDirection == FadeDirection.In ? 1f : 0f;
        float fadeStep = (Time.unscaledDeltaTime / fadeTime) * (fadeDirection == FadeDirection.In ? 1 : -1);

        if (fadeDirection == FadeDirection.In)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while ((fadeDirection == FadeDirection.Out && startAlpha > endAlpha) ||
          (fadeDirection == FadeDirection.In && startAlpha < endAlpha))
        {
            canvasGroup.alpha = startAlpha;
            startAlpha += fadeStep;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (fadeDirection == FadeDirection.Out)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
