using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fading : MonoBehaviour
{
    private SpriteRenderer sr;
    private float fadetime  = 3f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public IEnumerator StartFading()
    {
        float initialFadeTime = fadetime;

        while (fadetime > 0)
        {
            fadetime -= Time.deltaTime;

            if (sr.color.a > 0)
            {
                Color newColor = sr.color;
                newColor.a -= Time.deltaTime / initialFadeTime;
                sr.color = newColor;
                yield return null;
            }
        }

        gameObject.SetActive(false);
    }
}
