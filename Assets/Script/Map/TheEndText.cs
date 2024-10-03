using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEndText : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(DelayAnim());
    }


    IEnumerator DelayAnim()
    {
        yield return new WaitForSeconds(1);
        anim.SetTrigger("Trigger");
    }
}
