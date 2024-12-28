using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueFadeUI : FadeUI
{
    private CanvasGroup canvasPanel;

    private void Start()
    {
        canvasPanel = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeIn(float delay)
    {
        FadeUIIn(canvasPanel,delay);
        yield return new WaitForSeconds(delay);
    }
    public IEnumerator FadeOut(float delay)
    {
        FadeUIOut(canvasPanel,delay);
        yield return new WaitForSeconds(delay);
    }
}
