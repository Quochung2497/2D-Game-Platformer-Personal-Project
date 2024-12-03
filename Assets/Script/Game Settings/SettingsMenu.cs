using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI setting")]
    public GameObject[] menuButtons; // Các nút trong menu chính
    public CanvasGroup[] SubMenu;
    public float shrinkSize = 50f; // Kích thước khi thu gọn (chỉ hiện Icon)
    public float expandSize = 200f; // Kích thước khi mở rộng (cả Icon và Text)
    public float animationTime = 0.5f; // Thời gian để thực hiện animation

    public int currentButtonIndex = 0; // Lưu lại nút nào đang được chọn
    public int currentPanel = 0;
    public int currentButton = 0;
    [Header("Audio Setting")]
    [SerializeField] AudioMixer audioMixer;

    private bool onSubPanel = false;
    private Button[] button;

    void Start()
    {
        /*
        for (int i = 0; i < menuButtons.Length; i++)
        {
            int index = i; // Lưu chỉ số hiện tại vào biến local để sử dụng trong lambda
            EventTrigger trigger = menuButtons[i].gameObject.AddComponent<EventTrigger>();

            // Tạo sự kiện khi con trỏ chuột vào
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnButtonHover(index); });
            trigger.triggers.Add(entryEnter);
        }*/
        button = new Button[menuButtons.Length];
        for (int i = 0; i < menuButtons.Length; i++)
        {
            button[i] = menuButtons[i].GetComponent<Button>();
            if (button[i] == null)
            {
                Debug.LogError($"menuButtons[{i}] does not have a Button component.");
            }
        }
    }
   
    /*private void Update()
    {
        if (MenuController.Handle)
        {
            HandleInput();
        }
    }*/

    public void HandleInput()
    {
           
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                if(currentButtonIndex == 7)
                {
                    MenuSelection.instance.FadeToMenu();
                }
                /*else if (currentButtonIndex == -1)
                {
                    StartCoroutine(FadeOut(SubMenu[currentPanel], 0.5f, 0.5f));
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(menuButtons[lastButton]);
                }*/
                else
                {
                    /*OnButtonClick(currentButtonIndex);
                    ChangePanel(currentButtonIndex);*/
                    if (currentButtonIndex >= 0 && currentButtonIndex < button.Length)
                    {
                        button[currentButtonIndex].onClick.Invoke();
                    }
                    else
                    {
                        Debug.LogError("Invalid currentButtonIndex: " + currentButtonIndex);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                if (!onSubPanel)
                {
                    //MenuController.instance.ReturnToPreviousState();
                    MenuController.instance.MainMenuState();
                }
                else
                {
                    ExpandAllButtons();
                    onSubPanel = false;
                }
            }
    }



    public void SetVolume(float _volume)
    {
        audioMixer.SetFloat("Volume", _volume);
    }
    public void SetQuality(int _qualityIndex)
    {
        Debug.Log("Quality Index: " + _qualityIndex);
        QualitySettings.SetQualityLevel(_qualityIndex);
    }
    public void SetFullScreen(bool _isFullScreen)
    {
        Screen.fullScreen = _isFullScreen;
    }
    public void ShrinkAllButtons(int excludeIndex = -1)
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
                RectTransform buttonRect = menuButtons[i].GetComponent<RectTransform>();
                TextMeshProUGUI buttonText = menuButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                // Thu gọn nút chỉ còn Icon
                LeanTween.size(buttonRect, new Vector2(shrinkSize, buttonRect.sizeDelta.y), animationTime).setEase(LeanTweenType.easeInOutSine);

                // Ẩn Text
                if (buttonText != null)
                {
                    LeanTween.value(gameObject, 1f, 0f, animationTime).setOnUpdate((float val) =>
                    {
                        buttonText.alpha = val; // Giảm alpha của text về 0
                    });
                }
        }
    }
    // Hàm để mở rộng tất cả các nút
    public void ExpandAllButtons()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            RectTransform buttonRect = menuButtons[i].GetComponent<RectTransform>();
            TextMeshProUGUI buttonText = menuButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            // Mở rộng nút ra để hiện cả Icon và Text
            LeanTween.size(buttonRect, new Vector2(expandSize, buttonRect.sizeDelta.y), animationTime).setEase(LeanTweenType.easeInOutSine);

            // Hiển thị Text
            if (buttonText != null)
            {
                LeanTween.value(gameObject, 0f, 1f, animationTime).setOnUpdate((float val) =>
                {
                    buttonText.alpha = val; // Tăng alpha của text về 1
                });
            }
        }
    }

    // Hàm gọi khi nhấn vào một nút, để mở Submenu
    public void OnButtonClick(int Index)
    {

        /*if (currentButtonIndex == Index) // Nếu đang chọn nút hiện tại, thì quay lại menu chính
        {
            if (!FirstTime)
            {
                ExpandAllButtons(); // Mở rộng tất cả các nút
                currentButtonIndex = -1;
            }
            else
            {
                ShrinkAllButtons(Index); // Thu gọn tất cả ngoại trừ nút đang chọn
                currentButtonIndex = Index; // Ghi lại nút hiện tại
                FirstTime = false;
            }
        }
        else
        {
            ShrinkAllButtons(Index); // Thu gọn tất cả ngoại trừ nút đang chọn
            currentButtonIndex = Index; // Ghi lại nút hiện tại
            FirstTime = false;
        }*/
        //if (MenuController.instance.MenuState == Menu)
        {
            ShrinkAllButtons(Index); // Thu gọn tất cả ngoại trừ nút đang chọn
            currentButtonIndex = Index; // Ghi lại nút hiện tại
        }
    }
    IEnumerator FadeOut(CanvasGroup canvasGroup, float _seconds)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        yield return null;
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup, float _seconds)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / _seconds;
            yield return null;
        }

        yield return null;
    }
    public void ChangePanel(int newPanelIndex)
    {
        if (currentPanel != newPanelIndex) // Nếu nút mới khác nút hiện tại
        {
            StartCoroutine(FadeOut(SubMenu[currentPanel], 0.5f));
            StartCoroutine(FadeIn(SubMenu[newPanelIndex], 0.5f)); // Fade in panel mới
            currentPanel = newPanelIndex; // Cập nhật lại nút hiện tại
        }
        /*else
        {
            
            if(currentButtonIndex != -1)
            {
                //currentButtonIndex = lastButton;
                StartCoroutine(FadeOut(SubMenu[currentPanel], 0.5f, 0.5f));
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(menuButtons[currentButtonIndex]);
            }
        }*/
        onSubPanel = true;
    }
    //Dùng để fix di chuyển bằng analog
    /*
    void MoveSelectionBorder()
    {
        // Di chuyển Border hoặc Icon tới vị trí của nút được chọn
        selectionBorder.position = menuButtons[currentIndex].transform.position;
        // Điều chỉnh kích thước Border hoặc Icon để ôm vừa nút hiện tại
        selectionBorder.sizeDelta = menuButtons[currentIndex].GetComponent<RectTransform>().sizeDelta;
    }*/
}
