using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuState 
{ 
    MainMenu, 
    Settings, 
    InGame 
}
public class MenuController : MonoBehaviour
{
    public MenuSelection Menu;
    public SettingsMenu settingMenu;

    public static MenuController instance;

    [Header("MainMenu")]
    public CanvasGroup PressAnyCanvasGroup;
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup CreditCanvasGroup;

    [Header("SettingMenu")]
    public CanvasGroup settingsMenuCanvasGroup;
    
    private MenuState currentMenuState;
    private MenuState previousState;

    void Start()
    {
        // Khởi tạo trạng thái ban đầu của Menu
        currentMenuState = MenuState.MainMenu;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        HandleInput();
    }
    private void HandleInput()
    {
        switch (currentMenuState)
        {
            case MenuState.MainMenu:
                Menu?.HandleInput();
                break;

            case MenuState.Settings:
                settingMenu?.HandleInput();  
                break;

            case MenuState.InGame:
                
                break;
        }
    }
    public void MainMenuState()
    {
        if (currentMenuState != MenuState.MainMenu)
        {
            previousState = currentMenuState;
        }
        currentMenuState = MenuState.MainMenu;
        MenuSelection.instance.FadeToMenu();
    }
    public void InGameMenu()
    {
        currentMenuState = MenuState.InGame;
    }    
    public void SettingsMenu()
    {
        //previousState = currentMenuState;
        currentMenuState = MenuState.Settings;
    }
    public void ReturnToPreviousState()
    {
        Debug.Log("ReturnToPreviousState Called. PreviousState: " + previousState);
        currentMenuState = previousState;
        if (previousState == MenuState.InGame)
        {
            // Logic cho InGame state
        }
        else if (previousState == MenuState.MainMenu)
        {
            MenuSelection.instance.FadeToMenu();
            Debug.Log("Returned to MainMenu");
        }
    }
}
