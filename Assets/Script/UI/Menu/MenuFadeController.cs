using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuFadeController : MonoBehaviour
{
    private FadeUI fadeUI;
    [SerializeField] private float fadeTime;
    [SerializeField] private CanvasGroup canvasGroup;

    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        fadeUI.FadeUIOut(canvasGroup, fadeTime);
    }
    public void CallFadeAndStartGame(string _sceneToLoad)
    {
        StartCoroutine(FadeAndStartGame(_sceneToLoad));
    }

    IEnumerator FadeAndStartGame(string _sceneToLoad)
    {
        fadeUI.FadeUIIn(canvasGroup, fadeTime);
        PlayerPrefs.DeleteKey("playerScore");
        PlayerPrefs.Save();
        string playerDataPath = Application.persistentDataPath + "/save.player.data";
        string benchDataPath = Application.persistentDataPath + "/save.bench.data";
        string shadeDataPath = Application.persistentDataPath + "/save.shade.data";

        // Xóa tệp save.player.data
        if (File.Exists(playerDataPath))
        {
            File.Delete(playerDataPath);
            Debug.Log("File deleted successfully: " + playerDataPath);
        }
        else
        {
            Debug.Log("File not found: " + playerDataPath);
        }

        // Xóa tệp save.bench.data
        if (File.Exists(benchDataPath))
        {
            File.Delete(benchDataPath);
            Debug.Log("File deleted successfully: " + benchDataPath);
        }
        else
        {
            Debug.Log("File not found: " + benchDataPath);
        }

        // Xóa tệp save.shade.data
        if (File.Exists(shadeDataPath))
        {
            File.Delete(shadeDataPath);
            Debug.Log("File deleted successfully: " + shadeDataPath);
        }
        else
        {
            Debug.Log("File not found: " + shadeDataPath);
        }
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(_sceneToLoad);
    }
}
