using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEndScene : MonoBehaviour
{
    Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(DelayAnim());
    }


    IEnumerator DelayAnim()
    {
        yield return new WaitForSeconds(6);
        anim.SetTrigger("Trigger");
    }

}
