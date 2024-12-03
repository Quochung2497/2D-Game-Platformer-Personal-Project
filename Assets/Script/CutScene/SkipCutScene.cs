using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SkipCutScene : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> skipConditionBehaviours;
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI fillText;

    private List<ISkipCondition> skipConditions = new List<ISkipCondition>();
    private Coroutine fadeCoroutine;
    private CutSceneManager cutsceneManager;
    private float skipTimer = 0f;
    private const float skipThreshold = 2f;
    private bool isSkipped;

    void Start()
    {
        cutsceneManager = GetComponent<CutSceneManager>();
        isSkipped = false;
        foreach (var behaviour in skipConditionBehaviours)
        {
            if (behaviour is ISkipCondition condition)
            {
                skipConditions.Add(condition);
            }
            else
            {
                Debug.LogError($"The assigned behaviour {behaviour} does not implement ISkipCondition!");
            }
        }

        if (fill != null)
        {
            fill.fillAmount = 0f;
            if(fillText != null)
            {
                Color textColor = fillText.color;
                textColor.a = 0;
                fillText.color = textColor;
            }
        }
    }

    void Update()
    {
        bool isSkipping = skipConditions.Any(condition => condition.ShouldSkip());

        if (isSkipping)
        {
            skipTimer += Time.deltaTime;
            skipTimer = Mathf.Clamp(skipTimer, 0, skipThreshold);
            StartCoroutine(SetAlpha(1f, 0.2f, targetText: fillText));
            if (fill != null)
            {
                fill.fillAmount = skipTimer / skipThreshold;
            }

            if (Mathf.Approximately(fill.fillAmount, 1f))
            {
                //StopCoroutine(fadeCoroutine);
                isSkipped = true;
                StartCoroutine(SkipScene());
            }
        }
        else
        {
            if (!isSkipped)
            {
                skipTimer = Mathf.Max(skipTimer - Time.deltaTime, 0);
                if (fill != null)
                {
                    fill.fillAmount = skipTimer / skipThreshold;
                    if(fill.fillAmount == 0f)
                    {
                        fadeCoroutine = StartCoroutine(SetAlpha(0f, 0.5f, targetText: fillText));
                    }
                }
            }
        }
    }

    private IEnumerator SkipScene()
    {
        yield return SetAlpha(0f, 1f, targetImage: fill, targetText: fillText);
        cutsceneManager.SkipCutScene();
    }

    private IEnumerator SetAlpha(float targetAlpha, float duration, Image targetImage = null, TextMeshProUGUI targetText = null)
    {
        float startAlphaImage = targetImage != null ? targetImage.color.a : 0f;
        float startAlphaText = targetText != null ? targetText.color.a : 0f;

        float alphaTimer = 0f;

        while (alphaTimer < duration)
        {
            alphaTimer += Time.deltaTime;
            float newAlphaImage = Mathf.Lerp(startAlphaImage, targetAlpha, alphaTimer / duration);
            float newAlphaText = Mathf.Lerp(startAlphaText, targetAlpha, alphaTimer / duration);

            if (targetImage != null)
            {
                Color imageColor = targetImage.color;
                imageColor.a = newAlphaImage;
                targetImage.color = imageColor;
            }

            if (targetText != null)
            {
                Color textColor = targetText.color;
                textColor.a = newAlphaText;
                targetText.color = textColor;
            }

            yield return null;
        }

        if (targetImage != null)
        {
            Color imageColor = targetImage.color;
            imageColor.a = targetAlpha;
            targetImage.color = imageColor;
        }

        if (targetText != null)
        {
            Color textColor = targetText.color;
            textColor.a = targetAlpha;
            targetText.color = textColor;
        }
    }
}
