using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSelection : MonoBehaviour
{
    public RectTransform selectionBorder; // Border hoặc Icon
    public Button[] menuButtons;
    public CanvasGroup[] Menu;
    public GameObject leftBar;  // Tham chiếu đến GameObject của Select Bar bên trái
    public GameObject rightBar; // Tham chiếu đến GameObject của Select Bar bên phải
    public float moveDistance = 10f; // Khoảng cách di chuyển
    public float speed = 1f; // Tốc độ di chuyển
    //Biến toàn cục
    private int currentIndex = 0;
    private float inputDelay = 0.2f; // Thời gian chờ giữa các lần nhận input từ analog stick
    private float lastInputTime;
    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    //Biến cục bộ

    void Start()
    {
        // Gán sự kiện hover cho mỗi nút
        for (int i = 0; i < menuButtons.Length; i++)
        {
            int index = i; // Lưu chỉ số hiện tại vào biến local để sử dụng trong lambda
            EventTrigger trigger = menuButtons[i].gameObject.AddComponent<EventTrigger>();
            
            // Tạo sự kiện khi con trỏ chuột vào
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnButtonHover(index); });
            trigger.triggers.Add(entryEnter);
        }

        // Đặt Border ban đầu quanh nút đầu tiên
        MoveSelectionBorder();

        // Lưu vị trí ban đầu của các Select Bar
        leftStartPos = leftBar.transform.localPosition;
        rightStartPos = rightBar.transform.localPosition;

        // Bắt đầu Coroutine để thực hiện di chuyển liên tục
        StartCoroutine(MoveBars());
    }

    void Update()
    {
        HandleInput();

    }

    void HandleInput()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float dPagInput = Input.GetAxis("7th axis");
        bool moved = false;
        if (Menu[0].alpha == 1f)
        {
            // Kiểm tra nếu người dùng di chuyển xuống dưới
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || ((dPagInput < -0.5f) && Time.time - lastInputTime > inputDelay) || verticalInput < -0.5f) && Time.time - lastInputTime > inputDelay)
            {
                currentIndex = (currentIndex + 1) % menuButtons.Length;
                MoveSelectionBorder();
                lastInputTime = Time.time;
                moved = true;
            }
            // Kiểm tra nếu người dùng di chuyển lên trên
            else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || ((dPagInput > 0.5f) && Time.time - lastInputTime > inputDelay) || verticalInput > 0.5f) && Time.time - lastInputTime > inputDelay)
            {
                currentIndex = (currentIndex - 1 + menuButtons.Length) % menuButtons.Length;
                MoveSelectionBorder();
                lastInputTime = Time.time;
                moved = true;
            }

            // Kiểm tra nút bấm trên tay cầm DualSense 5 (nút X)
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                //menuButtons[currentIndex].onClick.Invoke();
                StartCoroutine(Click());
            }

            // Reset thời gian chờ nếu người dùng đã di chuyển
            if (moved)
            {
                lastInputTime = Time.time;
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Escape)|| Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                if (Menu[1].alpha == 1f)
                {
                    StartCoroutine(FadeOut(Menu[1], 0.5f));
                }
                else if (Menu[2].alpha == 1f)
                {
                    StartCoroutine(FadeOut(Menu[2], 0.5f));
                }
                StartCoroutine(FadeIn(Menu[0], 0.5f));
            }
        }
    }

    IEnumerator Click()
    {
        Animator left = leftBar.gameObject.GetComponent<Animator>();
        Animator right = rightBar.gameObject.GetComponent<Animator>();
        left.SetBool("Click", true);
        right.SetBool("Click", true);
        yield return new WaitForSeconds(0.3f);
        menuButtons[currentIndex].onClick.Invoke();
    }

    void OnButtonHover(int index)
    {
        currentIndex = index;
        MoveSelectionBorder();
    }

    void MoveSelectionBorder()
    {
        // Di chuyển Border hoặc Icon tới vị trí của nút được chọn
        selectionBorder.position = menuButtons[currentIndex].transform.position;
        // Điều chỉnh kích thước Border hoặc Icon để ôm vừa nút hiện tại
        selectionBorder.sizeDelta = menuButtons[currentIndex].GetComponent<RectTransform>().sizeDelta;
    }
    IEnumerator MoveBars()
    {
        while (true)
        {
            // Di chuyển ra xa
            yield return MoveBarsToPosition(moveDistance);
            // Di chuyển trở lại vị trí ban đầu
            yield return MoveBarsToPosition(-moveDistance);
        }
    }

    IEnumerator MoveBarsToPosition(float targetOffset)
    {
        float elapsedTime = 0f;
        Vector3 leftTargetPos = leftStartPos + new Vector3(targetOffset, 0, 0);
        Vector3 rightTargetPos = rightStartPos + new Vector3(-targetOffset, 0, 0);

        while (elapsedTime < speed)
        {
            leftBar.transform.localPosition = Vector3.Lerp(leftBar.transform.localPosition, leftTargetPos, elapsedTime / speed);
            rightBar.transform.localPosition = Vector3.Lerp(rightBar.transform.localPosition, rightTargetPos, elapsedTime / speed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo vị trí cuối cùng đúng
        leftBar.transform.localPosition = leftTargetPos;
        rightBar.transform.localPosition = rightTargetPos;
    }
    IEnumerator FadeOut(CanvasGroup canvasGroup, float _seconds)
    {
        Animator left = leftBar.gameObject.GetComponent<Animator>();
        Animator right = rightBar.gameObject.GetComponent<Animator>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        left.SetBool("Click", false);
        right.SetBool("Click", false);
        yield return null;
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup, float _seconds)
    {
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return null;
    }
}
