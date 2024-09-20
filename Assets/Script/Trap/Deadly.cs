using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadly : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.collider.gameObject.layer);

        if (layerName == "Player")
        {
            PlayerController playerController = collision.collider.GetComponent<PlayerController>();
            playerController.TakeDamage(playerController.health);
        }
        else if (layerName == "Attackable")
        {
            Enemy enemyController = collision.collider.GetComponent<Enemy>();
            enemyController.TrapHit(100f);
        }
    }
}
