using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FinalBoss_Event : MonoBehaviour
{
    void SlashDamagePlayer()
    {
        float playerDistance = Vector2.Distance(PlayerController.Instance.transform.position, transform.position);
        float xDifference = PlayerController.Instance.transform.position.x - transform.position.x;
        float yDifference = PlayerController.Instance.transform.position.y - transform.position.y;
        if (Mathf.Abs(xDifference) > Mathf.Abs(yDifference))
        {
            Hit(FinalBoss.Instance.SideAttackTransform, FinalBoss.Instance.SideAttackArea);
        }
        else if (yDifference > 0)
        {
            Hit(FinalBoss.Instance.UpAttackTransform, FinalBoss.Instance.UpAttackArea);
        }
        else if (yDifference < 0)
        {
            Hit(FinalBoss.Instance.DownAttackTransform, FinalBoss.Instance.DownAttackArea);
        }
    }
    void Hit(Transform _attackTransform, float _attackrange)
    {
        LayerMask playerLayerMask = LayerMask.GetMask("Player");

        // Use OverlapCircle with the correct layer mask
        Collider2D _objectsToHit = Physics2D.OverlapCircle(_attackTransform.position, _attackrange, playerLayerMask);

        if (_objectsToHit != null)
        {
            PlayerController player = _objectsToHit.GetComponent<PlayerController>();

            if (player != null && !player.pState.Invincible)
            {
                player.TakeDamage(FinalBoss.Instance.damage);
                if (player.pState.alive)
                {
                    player.HitStopTime(0, 5, 0.25f);
                }
            }
            else
            {
                Debug.Log("PlayerController not found or player is invincible.");
            }
        }
        else
        {
            Debug.Log("No objects hit within the attack area.");
        }
    }
    void Parrying()
    {
        if (!FinalBoss.Instance.parrying)
        {
            FinalBoss.Instance.parrying = true;
            Debug.Log("Parrying set to true by animation event");
        }
    }
    void BendDownCheck()
    {
        if (FinalBoss.Instance.barrageAttack)
        {
            StartCoroutine(BarrageAttackTransition());
        }
        if (FinalBoss.Instance.outbreakAttack)
        {
            StartCoroutine(OutbreakAttackTransition());
        }
        if (FinalBoss.Instance.bounceAttack)
        {
            FinalBoss.Instance._animator.SetTrigger("Bounce1");
        }
    }

    void BarrageOrOutbreak()
    {
        if (FinalBoss.Instance.barrageAttack)
        {
            FinalBoss.Instance.StartCoroutine(FinalBoss.Instance.Barrage());
        }
        if (FinalBoss.Instance.outbreakAttack)
        {
            FinalBoss.Instance.StartCoroutine(FinalBoss.Instance.Outbreak());
        }
    }
    IEnumerator OutbreakAttackTransition()
    {
        yield return new WaitForSeconds(0.1f);
        FinalBoss.Instance.Flip();
        FinalBoss.Instance._animator.SetBool("Cast", true);
    }
    IEnumerator BarrageAttackTransition()
    {
        yield return new WaitForSeconds(0.1f);
        FinalBoss.Instance.Flip();
        FinalBoss.Instance._animator.SetBool("Barrage", true);
    }
    void DestroyDeath()
    {
        FinalBoss.Instance.StartCoroutine(FinalBoss.Instance.DestroyAfterDeath());
    }
    void PlayDeath()
    {
        FinalBoss.Instance.PlayDeathSound();
    }
    void PlayIntro()
    {
        FinalBoss.Instance.PlayIntroSound();
    }
    void PlaySlash3()
    {
        FinalBoss.Instance.PlaySlash3Sound();
    }
    void PlaySlash()
    {
        FinalBoss.Instance.PlaySlashSound();
    }
    void PlayParry()
    {
        FinalBoss.Instance.PlayParrySound();
    }
    void PlayDive()
    {
        FinalBoss.Instance.PlayDiveSound();
    }
    void PlayDash()
    {
        FinalBoss.Instance.PlayDashSound();
    }
    void PlayBarrage()
    {
        FinalBoss.Instance.PlayBarrageSound();
    }
    void BeginParry()
    {
        FinalBoss.Instance.StartParry();
    }
    void EndParry()
    {
        FinalBoss.Instance.StopParry();
    }
}
