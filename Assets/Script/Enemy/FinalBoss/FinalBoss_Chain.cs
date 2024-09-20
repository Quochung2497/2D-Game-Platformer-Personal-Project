using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss_Chain : MonoBehaviour
{

    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void PlayAnimation()
    {
        anim.SetTrigger("Trigger");
    }
}
