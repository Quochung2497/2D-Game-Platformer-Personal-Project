using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivePillar : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.Invincible)
        {
            _other.GetComponent<PlayerController>().TakeDamage(FinalBoss.Instance.damage);
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.25f);
            }
        }
    }
}
