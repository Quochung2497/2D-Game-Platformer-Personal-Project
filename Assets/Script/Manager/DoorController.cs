using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public static DoorController doorcontroller;
    private Animator anim;
    public GameObject chargeEnemy, chargeEnemy1;
    private BoxCollider2D originalBoxCollider;
    private bool activeCoroutineStarted = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Awake()
    {
        if (doorcontroller != null && doorcontroller != this)
        {
            Destroy(gameObject);
        }
        else
        {
            doorcontroller = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    public virtual void OpeningDoor()
    {
        anim.SetBool("isOpening", true);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            originalBoxCollider = box;
            Destroy(box);
        }

        if (chargeEnemy != null)
        {
            StartCoroutine(active());
        }
        StartCoroutine(RecreateBoxColliderAfterDelay(5f));
    }

    public virtual void OpenDoor()
    {
        anim.SetBool("isOpening", true);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            originalBoxCollider = box;
            Destroy(box);
        }
    }
    IEnumerator active()
    {

        yield return new WaitForSeconds(3);

        // Luôn kích hoạt chargeEnemy
        if (chargeEnemy != null)
        {
            chargeEnemy.SetActive(true);
            activeCoroutineStarted = true; // Đánh dấu rằng coroutine đã được bắt đầu
            Debug.Log("activeCoroutine");
        }

        // Đầu ra gỡ lỗi
        Debug.Log($"ChargeEnemy Active: {chargeEnemy?.activeInHierarchy}");
    }
    IEnumerator RecreateBoxColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetBool("isClosing", true);
        BoxCollider2D newBoxCollider = gameObject.AddComponent<BoxCollider2D>();
        if (originalBoxCollider != null)
        {
            newBoxCollider.offset = originalBoxCollider.offset;
            newBoxCollider.size = originalBoxCollider.size;
            newBoxCollider.isTrigger = originalBoxCollider.isTrigger;
        }
    }
    private void Update()
    {
        // Chỉ kiểm tra nếu coroutine đã được bắt đầu
        if (activeCoroutineStarted)
        {
            // Liên tục kiểm tra trạng thái của chargeEnemy
            if (chargeEnemy == null)
            {
                Debug.Log("ChargeEnemy is inactive. Activating ChargeEnemy1.");

                if (chargeEnemy1 != null)
                {
                    chargeEnemy1.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("ChargeEnemy1 is null.");
                }

                // Đặt lại activeCoroutineStarted để tránh kiểm tra liên tục sau khi chargeEnemy1 đã được kích hoạt
                activeCoroutineStarted = false;
            }
        }
    }
}
