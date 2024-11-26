using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public List<GameObject> dialogueObjects;
    public StoryElement story;

    [SerializeField] private float delayBetweenSentences = 1.0f;
    [SerializeField] private float delay = 0.01f;

    private int currentDialogueIndex = 0;
    private bool isCompleted = false;

    private void Start()
    {
        foreach (GameObject obj in dialogueObjects)
        {
            obj.SetActive(false);
        }

        if (dialogueObjects.Count > 0 && story != null && story.sentences.Length > 0)
        {
            StartCoroutine(DisplayNextSentence());
        }
    }

    public IEnumerator DisplayNextSentence()
    {
        while (currentDialogueIndex < dialogueObjects.Count && currentDialogueIndex < story.sentences.Length)
        {
            GameObject currentObject = dialogueObjects[currentDialogueIndex];
            TextMeshProUGUI textComponent = currentObject.GetComponentInChildren<TextMeshProUGUI>();

            currentObject.SetActive(true);

            if (textComponent != null)
            {
                textComponent.text = "";

                foreach (char c in story.sentences[currentDialogueIndex].ToCharArray())
                {
                    textComponent.text += c;
                    yield return new WaitForSeconds(delay);
                }
            }

            yield return new WaitForSeconds(delayBetweenSentences);
            currentDialogueIndex++;
        }
        StartCoroutine(CloseAllText());
    }

    public IEnumerator DisplayNextManually()
    {
        if (currentDialogueIndex < dialogueObjects.Count && currentDialogueIndex < story.sentences.Length)
        {
            GameObject currentObject = dialogueObjects[currentDialogueIndex];
            TextMeshProUGUI textComponent = currentObject.GetComponentInChildren<TextMeshProUGUI>();

            currentObject.SetActive(true);

            if (textComponent != null)
            {
                textComponent.text = "";

                foreach (char c in story.sentences[currentDialogueIndex].ToCharArray())
                {
                    textComponent.text += c;
                    yield return new WaitForSeconds(delay);
                }
            }

            currentObject.SetActive(true);

            currentDialogueIndex++;
        }
    }
    public IEnumerator CloseAllText()
    {
        yield return new WaitForSeconds (delayBetweenSentences);
        foreach (GameObject obj in dialogueObjects)
        {
            TextMeshProUGUI textComponent = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                yield return FadeOutText(textComponent, 0.2f);
            }
            obj.SetActive(false);
        }
        Debug.Log("End");
        isCompleted = true;
    }
    private IEnumerator FadeOutText(TextMeshProUGUI textComponent, float duration)
    {
        float startAlpha = textComponent.color.a; 
        float elapsedTime = 0f;

        Color currentColor = textComponent.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / duration);
            textComponent.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
        textComponent.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
    }
    public bool IsCompleted()
    {
        return isCompleted;
    }
}

