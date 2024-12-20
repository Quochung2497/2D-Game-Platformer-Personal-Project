using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFadeUI : FadeUI
{
    [SerializeField] private float fadeTime;

    void Start()
    {
        FadeUIOut(fadeTime);
    }
    public IEnumerator FadeIn()
    {
        FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
    }
}
