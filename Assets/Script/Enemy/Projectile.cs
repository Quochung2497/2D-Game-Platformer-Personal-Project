using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Trap
{
    public Vector2 direction;
    public int damageToPlayer;
    public float movingSpeed;
    public float destroyTime;

    public AudioClip ImpactSound;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.collider.gameObject.layer);

        if (layerName == "Player" && !PlayerController.Instance.pState.Invincible)
        {
            PlayerController playerController = collision.collider.GetComponent<PlayerController>();
            if (PlayerController.Instance.pState.alive && !PlayerController.Instance.pState.Invincible && !PlayerController.Instance.pState.cutscenes)
            {
                audioSource.PlayOneShot(ImpactSound);
                PlayerController.Instance.HitStopTime(0, 5, 0.25f);
                PlayerController.Instance.TakeDamage(damageToPlayer);
            }
            Destroy(gameObject);
        }
        else if(layerName == "Player" && PlayerController.Instance.pState.Invincible)
        {
            Destroy(gameObject);
        }
    }

    public override void trigger()
    {
        Vector2 newVelocity = direction.normalized * movingSpeed;
        gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
        StartCoroutine(destroyCoroutine(destroyTime));
    }

    private IEnumerator destroyCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
