using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public List<GameObject> dialogueObjects;
    public StoryElement story;

    [SerializeField] private float delayBetweenSentences = 1.0f;
    [SerializeField] private float delay = 0.01f;

    [Header("Fast Forward")]
    [SerializeField] private KeyCode fastForwardKey = KeyCode.Space;
    [SerializeField, Min(1f)] private float fastForwardMultiplier = 8f;
    [SerializeField] private CanvasGroup fastForwardHint;

    private int currentDialogueIndex = 0;
    private bool isCompleted = false;
    private Coroutine storyCoroutine;
    private float hintStartedAt;

    private void Start()
    {
        foreach (GameObject obj in dialogueObjects)
        {
            obj.SetActive(false);
        }

        hintStartedAt = Time.unscaledTime;
        HideFastForwardHint();

        if (dialogueObjects.Count > 0 && story != null && story.sentences.Length > 0)
        {
            storyCoroutine = StartCoroutine(DisplayNextSentence());
        }
    }

    private void OnDisable()
    {
        if (storyCoroutine != null)
        {
            StopCoroutine(storyCoroutine);
            storyCoroutine = null;
        }

        HideFastForwardHint();
    }

    public IEnumerator DisplayNextSentence()
    {
        while (currentDialogueIndex < dialogueObjects.Count && currentDialogueIndex < story.sentences.Length)
        {
            GameObject currentObject = dialogueObjects[currentDialogueIndex];
            TextMeshProUGUI textComponent = currentObject.GetComponentInChildren<TextMeshProUGUI>(true);

            currentObject.SetActive(true);

            if (textComponent != null)
            {
                yield return TypeSentence(textComponent, story.sentences[currentDialogueIndex]);
            }

            yield return WaitForStoryTime(delayBetweenSentences);
            currentDialogueIndex++;
        }

        yield return CloseAllText();
        storyCoroutine = null;
    }

    public IEnumerator DisplayNextManually()
    {
        if (currentDialogueIndex < dialogueObjects.Count && currentDialogueIndex < story.sentences.Length)
        {
            GameObject currentObject = dialogueObjects[currentDialogueIndex];
            TextMeshProUGUI textComponent = currentObject.GetComponentInChildren<TextMeshProUGUI>(true);

            currentObject.SetActive(true);

            if (textComponent != null)
            {
                yield return TypeSentence(textComponent, story.sentences[currentDialogueIndex]);
            }

            currentDialogueIndex++;
        }
    }

    private IEnumerator TypeSentence(TextMeshProUGUI textComponent, string sentence)
    {
        Color color = textComponent.color;
        color.a = 1f;
        textComponent.color = color;
        textComponent.text = sentence;
        textComponent.maxVisibleCharacters = 0;
        textComponent.ForceMeshUpdate();

        int characterCount = textComponent.textInfo.characterCount;
        float elapsedTime = 0f;
        float duration = characterCount * delay;

        while (elapsedTime < duration)
        {
            elapsedTime += AdvanceStoryTime();
            textComponent.maxVisibleCharacters = Mathf.Min(
                characterCount,
                1 + Mathf.FloorToInt(elapsedTime / delay));
            yield return null;
        }

        textComponent.maxVisibleCharacters = characterCount;
    }

    private IEnumerator WaitForStoryTime(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += AdvanceStoryTime();
            yield return null;
        }
    }

    public IEnumerator CloseAllText()
    {
        yield return WaitForStoryTime(delayBetweenSentences);
        foreach (GameObject obj in dialogueObjects)
        {
            TextMeshProUGUI textComponent = obj.GetComponentInChildren<TextMeshProUGUI>(true);
            if (textComponent != null)
            {
                yield return FadeOutText(textComponent, 0.2f);
            }
            obj.SetActive(false);
        }

        HideFastForwardHint();
        isCompleted = true;
    }

    private IEnumerator FadeOutText(TextMeshProUGUI textComponent, float duration)
    {
        float elapsedTime = 0f;
        Color color = textComponent.color;
        float startAlpha = color.a;

        while (elapsedTime < duration)
        {
            elapsedTime += AdvanceStoryTime();
            color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            textComponent.color = color;
            yield return null;
        }

        color.a = 0f;
        textComponent.color = color;
    }

    private float AdvanceStoryTime()
    {
        bool isFastForwarding = Input.GetKey(fastForwardKey);

        if (fastForwardHint != null)
        {
            float visibleTime = Time.unscaledTime - hintStartedAt;
            float fadeIn = Mathf.Clamp01(visibleTime / 0.35f);
            float pulse = 0.55f + 0.25f * Mathf.Sin(visibleTime * Mathf.PI);
            fastForwardHint.alpha = fadeIn * (isFastForwarding ? 1f : pulse);
        }

        float speed = isFastForwarding ? Mathf.Max(1f, fastForwardMultiplier) : 1f;
        return Time.unscaledDeltaTime * speed;
    }

    private void HideFastForwardHint()
    {
        if (fastForwardHint != null)
        {
            fastForwardHint.alpha = 0f;
        }
    }

    public bool IsCompleted()
    {
        return isCompleted;
    }
}
