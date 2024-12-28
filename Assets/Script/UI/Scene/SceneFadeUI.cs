using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFadeUI : FadeUI
{
    [SerializeField] private float fadeTime;
    private CanvasGroup sceneFadeCanvas;

    void Start()
    {
        sceneFadeCanvas = GetComponent<CanvasGroup>();
        FadeUIOut(sceneFadeCanvas, fadeTime);
    }
    public IEnumerator FadeIn()
    {
        FadeUIIn(sceneFadeCanvas,fadeTime);
        yield return new WaitForSeconds(fadeTime);
    }
}
