using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupFade : MonoBehaviour
{
    [Header("FirsttimeMessage")]
    public GameObject notificationPanel;

    private int storeData = 0;

    private const string notificationKey = "Shown";

    [SerializeField] bool deleteStoredData = false;
    [Header("FadingSetting")]
    public CanvasGroup canvasGroup;
    public float fadeDuration = 2f;
    void Start()
    {
        canvasGroup.alpha = 0;
        StartCoroutine(FadeIn());
    }

    private void Awake()
    {
        storeData = PlayerPrefs.GetInt(notificationKey, 0);
        if (notificationPanel != null)
        {
            if (storeData == 0)
            {
                notificationPanel.SetActive(true);
                
            }
            else
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
        PlayerPrefs.SetInt(notificationKey, 1);
    }
    private void OnValidate()
    {
        if(deleteStoredData)
        {
            deleteStoredData = false;
            PlayerPrefs.DeleteKey(notificationKey);
            Debug.Log("Deleted");
        }
    }
    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;

    }

    public void Fade()
    {
        PlayerPrefs.SetInt(notificationKey, 1);
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            yield return null;
        }

        canvasGroup.alpha = 0;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main Menu");
    }
}
