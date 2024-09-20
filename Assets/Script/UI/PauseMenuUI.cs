using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;

    public bool GameIsPaused = false;

    [SerializeField] FadeUI pauseMenu,OptionMenu;
    [SerializeField] float fadeTime;
    void Awake()
    {
        // Check if there's already an instance of this object
        if (Instance == null)
        {
            // If not, set this as the instance and mark it as DontDestroyOnLoad
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If there's already an instance, destroy this object to prevent duplicates
            Destroy(gameObject);
        }
        if (pauseMenu != null)
        {
            DontDestroyOnLoad(pauseMenu);
        }
        else
        {
            Destroy(pauseMenu);
        }
    }

    // Update is called once per frame
    public void Pause()
    {
        pauseMenu.FadeUIIn(fadeTime);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void SaveGame()
    {
        SaveData.Instance.SavePlayerData();
    }
    public void Resume()
    {
        pauseMenu.FadeUIOut(fadeTime);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Option()
    {
        pauseMenu.FadeUIOut(fadeTime);
        OptionMenu.FadeUIIn(fadeTime);
    }
    public void Back()
    {
        OptionMenu.FadeUIOut(fadeTime);
        pauseMenu.FadeUIIn(fadeTime);
    }
    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
