using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartShards : MonoBehaviour
{
    public Image fill;
    public Animator heartAnimator;
    public float targetFillAmount;
    public float lerpDuration = 1.5f;
    public float initialFillAmount;
    private float nextAnimationThreshold;

    void Start()
    {
        nextAnimationThreshold = GetNextAnimationThreshold(initialFillAmount);
        fill.enabled = false;
    }

    public IEnumerator LerpFill()
    {
        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lerpDuration);

            float lerpedFillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, t);
            fill.fillAmount = lerpedFillAmount;
            if (lerpedFillAmount >= nextAnimationThreshold)
            {
                PlayFillAnimation(nextAnimationThreshold);
                nextAnimationThreshold = GetNextAnimationThreshold(nextAnimationThreshold);
            }
            yield return null;
        }

        fill.fillAmount = targetFillAmount;

        if (fill.fillAmount == 1)
        {
            PlayerController.Instance.maxHealth++;
            PlayerController.Instance.onHealthChangedCallback();
            PlayerController.Instance.heartShards = 0;
        }
    }
    void PlayFillAnimation(float fillAmount)
    {
        if (fillAmount == 0.25f)
        {
            heartAnimator.Play("Fill_25");
        }
        else if (fillAmount == 0.5f)
        {
            heartAnimator.Play("Fill_50");
        }
        else if (fillAmount == 0.75f)
        {
            heartAnimator.Play("Fill_75");
        }
        else if (fillAmount == 1f)
        {
            heartAnimator.Play("Fill_100");
        }
    }

    float GetNextAnimationThreshold(float currentAmount)
    {
        if (currentAmount < 0.25f)
            return 0.25f;
        else if (currentAmount < 0.5f)
            return 0.5f;
        else if (currentAmount < 0.75f)
            return 0.75f;
        else
            return 1f;
    }
}
