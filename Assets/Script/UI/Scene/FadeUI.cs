using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public virtual void FadeUIIn(float _seconds)
    {
        StartCoroutine(FadeIn(_seconds));
    }

    public virtual void FadeUIOut(float _seconds)
    {
        StartCoroutine(FadeOut(_seconds));
    }

    IEnumerator FadeOut(float _seconds)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        yield return null;
    }
    IEnumerator FadeIn(float _seconds)
    {
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return null;
    }
}
