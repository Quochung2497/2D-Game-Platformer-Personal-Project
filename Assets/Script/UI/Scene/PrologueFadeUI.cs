using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueFadeUI : FadeUI
{
    public IEnumerator FadeIn(float delay)
    {
        FadeUIIn(delay);
        yield return new WaitForSeconds(delay);
    }
    public IEnumerator FadeOut(float delay)
    {
        FadeUIOut(delay);
        yield return new WaitForSeconds(delay);
    }
}
