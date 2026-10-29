using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkipCutScene : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> skipConditionBehaviours;
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI fillText;

    private readonly List<ISkipCondition> skipConditions = new List<ISkipCondition>();
    private CutSceneManager cutsceneManager;
    private float skipTimer;
    private const float skipThreshold = 2f;
    private bool waitForRelease;

    private void Awake()
    {
        cutsceneManager = GetComponent<CutSceneManager>();
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
    }

    private void OnEnable()
    {
        cutsceneManager.SegmentChanged += OnSegmentChanged;
        OnSegmentChanged();
        SetHintAlpha(0f);
    }

    private void OnDisable()
    {
        cutsceneManager.SegmentChanged -= OnSegmentChanged;
        ResetProgress();
        SetHintAlpha(0f);
    }

    private void Update()
    {
        bool isHeld = IsSkipHeld();

        if (!cutsceneManager.CanSkip)
        {
            waitForRelease |= isHeld;
            ResetProgress();
        }
        else if (waitForRelease)
        {
            waitForRelease = isHeld;
            ResetProgress();
        }
        else if (isHeld)
        {
            skipTimer = Mathf.Min(skipTimer + Time.unscaledDeltaTime, skipThreshold);
            if (fill != null)
            {
                fill.fillAmount = skipTimer / skipThreshold;
            }

            if (skipTimer >= skipThreshold)
            {
                waitForRelease = true;
                ResetProgress();
                cutsceneManager.SkipCutScene();
            }
        }
        else
        {
            ResetProgress();
        }

        bool showHint = isHeld && !waitForRelease && cutsceneManager.CanSkip;
        if (fillText != null)
        {
            float targetAlpha = showHint ? 1f : 0f;
            float fadeSpeed = showHint ? 5f : 2f;
            SetHintAlpha(Mathf.MoveTowards(fillText.color.a, targetAlpha, Time.unscaledDeltaTime * fadeSpeed));
        }
    }

    private bool IsSkipHeld()
    {
        foreach (var condition in skipConditions)
        {
            if (condition.ShouldSkip()) return true;
        }

        return false;
    }

    private void OnSegmentChanged()
    {
        ResetProgress();
        waitForRelease = IsSkipHeld();
    }

    private void ResetProgress()
    {
        skipTimer = 0f;
        if (fill != null)
        {
            fill.fillAmount = 0f;
        }
    }

    private void SetHintAlpha(float alpha)
    {
        if (fillText == null) return;

        Color color = fillText.color;
        color.a = alpha;
        fillText.color = color;
    }
}
