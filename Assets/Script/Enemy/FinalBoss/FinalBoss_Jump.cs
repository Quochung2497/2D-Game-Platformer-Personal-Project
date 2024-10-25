using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss_Jump : StateMachineBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Light light;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        sr = animator.GetComponent<SpriteRenderer>();
        light = animator.GetComponent<Light>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DiveAttack();
    }

    void DiveAttack()
    {
        if (FinalBoss.Instance.diveAttack)
        {
            FinalBoss.Instance.Flip();
            Vector2 _newPos = Vector2.MoveTowards(rb.position, FinalBoss.Instance.moveToPosition,
            FinalBoss.Instance.speed * 8 * Time.fixedDeltaTime);
            rb.MovePosition(_newPos);

            if (FinalBoss.Instance.TouchedWall())
            {
                FinalBoss.Instance.moveToPosition.x = rb.position.x;
                _newPos = Vector2.MoveTowards(rb.position, FinalBoss.Instance.moveToPosition,
                    FinalBoss.Instance.speed * 8 * Time.fixedDeltaTime);
            }

            float _distance = Vector2.Distance(rb.position, _newPos);
            if (_distance < 0.05f)
            {
                FinalBoss.Instance.Dive();
            }
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
