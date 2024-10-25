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

    public CanvasGroup PressAnyCanvasGroup;
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup settingsMenuCanvasGroup;
    
    private MenuState currentMenuState;
    private MenuState previousState;
    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }
    private void HandleInput()
    {
        switch (currentMenuState)
        {
            case MenuState.MainMenu:
                // Kiểm tra nếu menuSelection đang hoạt động
                if (Menu != null && mainMenuCanvasGroup.alpha == 1 || PressAnyCanvasGroup.alpha == 1)
                {
                    Menu.HandleInput();  // Xử lý input cho MenuSelection
                }
                break;

            case MenuState.Settings:
                // Kiểm tra nếu settingsMenu đang hoạt động
                if (settingMenu != null && settingsMenuCanvasGroup.alpha == 1)
                {
                    settingMenu.HandleInput();  // Xử lý input cho SettingsMenu
                }
                break;

            case MenuState.InGame:
                // Khi đang trong game,tạm thời không cần xử lý menu
                
                break;
        }
    }
    public void MainMenuState()
    {
        previousState = currentMenuState;
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
        currentMenuState = previousState;
        if (previousState == MenuState.InGame)
        {
            
        }
        else if (previousState == MenuState.MainMenu)
        {
            MenuSelection.instance.FadeToMenu();
            Debug.Log("Ve MainMenu");
        }
    }
}
