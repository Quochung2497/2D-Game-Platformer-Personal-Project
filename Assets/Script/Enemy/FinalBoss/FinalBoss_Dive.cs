using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss_Dive : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool callOnce;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FinalBoss.Instance.divingCollider.SetActive(true);

        if (FinalBoss.Instance.Grounded())
        {
            FinalBoss.Instance.divingCollider.SetActive(false);
            FinalBoss.Instance.DiveActive();
            if (!callOnce)
            {
                FinalBoss.Instance.DivingPillars();
                animator.SetBool("Dive", false);
                FinalBoss.Instance.ResetAllAttacks();
                callOnce = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callOnce = false;
    }
}
