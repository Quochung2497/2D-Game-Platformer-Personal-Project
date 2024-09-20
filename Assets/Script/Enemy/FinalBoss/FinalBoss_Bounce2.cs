using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss_Bounce2 : StateMachineBehaviour
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
        Vector2 _forceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * FinalBoss.Instance.rotationDirectionToTarget),
        Mathf.Sin(Mathf.Deg2Rad * FinalBoss.Instance.rotationDirectionToTarget));
        rb.AddForce(_forceDirection * 1, ForceMode2D.Impulse);

        FinalBoss.Instance.divingCollider.SetActive(true);

        if (FinalBoss.Instance.Grounded())
        {
            FinalBoss.Instance.PlayBounceSound();
            FinalBoss.Instance.DiveActive();
            FinalBoss.Instance.divingCollider.SetActive(false);
            if (!callOnce)
            {
                FinalBoss.Instance.ResetAllAttacks();
                FinalBoss.Instance.CheckBounce();
                callOnce = true;
            }

            animator.SetTrigger("Idle");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Bounce2");
        animator.ResetTrigger("Idle");
        callOnce = false;
    }
}
