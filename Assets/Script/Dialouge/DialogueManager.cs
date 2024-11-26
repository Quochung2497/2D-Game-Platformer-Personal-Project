using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI Text;
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void BeginDialogue(Dialogue dialouge, float delay)
    {
        sentences.Clear();
        foreach(string sentence in dialouge.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplaySentence(delay);
    }

    public void DisplaySentence(float delay)
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeChar(sentence, delay));
    }

    private IEnumerator TypeChar(string sentence, float delay)
    {
        Text.text = "";
        foreach(char c in sentence.ToCharArray())
        {
            Text.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
    private void EndDialogue()
    {
        Debug.Log("End");
    }
}
