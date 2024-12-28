using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;

    public bool GameIsPaused = false;

    [SerializeField] FadeUI pauseMenu,OptionMenu;
    [SerializeField] private CanvasGroup pausePanel, optionPanel;
    [SerializeField] float fadeTime;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
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

    public void Pause()
    {
        pauseMenu.FadeUIIn(pausePanel, fadeTime);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void SaveGame()
    {
        SaveData.Instance.SavePlayerData();
    }
    public void Resume()
    {
        pauseMenu.FadeUIOut(pausePanel, fadeTime);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Option()
    {
        pauseMenu.FadeUIOut(pausePanel, fadeTime);
        OptionMenu.FadeUIIn(optionPanel,fadeTime);
    }
    public void Back()
    {
        OptionMenu.FadeUIOut(optionPanel, fadeTime);
        pauseMenu.FadeUIIn(pausePanel, fadeTime);
    }
    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
