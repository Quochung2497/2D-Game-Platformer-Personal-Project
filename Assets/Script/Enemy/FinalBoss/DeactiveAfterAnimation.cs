using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveAfterAnimation : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DisableAfterAnimation());
    }
    void Update()
    {
        StartCoroutine(DisableAfterAnimation());
    }

    IEnumerator DisableAfterAnimation()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Lấy thông tin về trạng thái hoạt hình hiện tại
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Chờ trong thời gian hoạt hình
            yield return new WaitForSeconds(stateInfo.length);

            // Đặt gameObject thành không hoạt động
            gameObject.SetActive(false);
        }
    }
}
