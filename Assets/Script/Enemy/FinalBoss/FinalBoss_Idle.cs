using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss_Idle : StateMachineBehaviour
{
    Rigidbody2D finabossRb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finabossRb = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finabossRb.velocity = Vector2.zero;
        RunToPlayer(animator);

        if (FinalBoss.Instance.attackCountdown <= 0)
        {
            FinalBoss.Instance.AttackHandler();
            FinalBoss.Instance.attackCountdown = Random.Range(FinalBoss.Instance.attackTimer - 1, FinalBoss.Instance.attackTimer + 1);
        }

        if (!FinalBoss.Instance.Grounded())
        {
            finabossRb.velocity = new Vector2(finabossRb.velocity.x, -25); //if knight is not grounded, fall to ground
        }
    }
    void RunToPlayer(Animator animator)
    {
        if (Vector2.Distance(PlayerController.Instance.transform.position, finabossRb.position) >= FinalBoss.Instance.attackRange)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            return;
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
