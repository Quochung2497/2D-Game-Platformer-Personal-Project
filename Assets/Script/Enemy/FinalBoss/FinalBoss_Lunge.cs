using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss_Lunge : StateMachineBehaviour
{
    Rigidbody2D finalbossRb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finalbossRb = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        finalbossRb.gravityScale = 0;
        int _dir = FinalBoss.Instance.facingRight ? 1 : -1;
        finalbossRb.velocity = new Vector2(_dir * (FinalBoss.Instance.speed * 5), 0f);

        if (Vector2.Distance(PlayerController.Instance.transform.position, finalbossRb.position) <= FinalBoss.Instance.attackRange &&
            !FinalBoss.Instance.damagedPlayer && !PlayerController.Instance.pState.Invincible)
        {
            PlayerController.Instance.TakeDamage(FinalBoss.Instance.damage);
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
            }
            FinalBoss.Instance.damagedPlayer = true;
        }
        if(FinalBoss.Instance.TouchedWall())
        {
            finalbossRb.velocity = new Vector2 (0,0);
            animator.SetBool("Lunge", false);
            FinalBoss.Instance.damagedPlayer = false;
            FinalBoss.Instance.Flip();
            FinalBoss.Instance.ResetAllAttacks();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
