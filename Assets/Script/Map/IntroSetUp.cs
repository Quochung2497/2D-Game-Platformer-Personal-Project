using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSetUp : MonoBehaviour
{
    private Animator animator;
    public GameObject Nextpanel;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayRunOutAnimation()
    {
        animator.SetTrigger("IntroOut");
        StartCoroutine(DeactivateAfterAnimationEnds());
    }

    private IEnumerator DeactivateAfterAnimationEnds()
    {
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength + 2f);
        gameObject.SetActive(false);
        Nextpanel.SetActive(true);
    }
}
