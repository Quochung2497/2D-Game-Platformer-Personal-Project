using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideSpell : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] float speed;
    [SerializeField] float lifetime = 1f;
    [Header("HitObjectVfx")]
    [SerializeField] private GameObject HitObjectSplashEffect;
    [SerializeField] private CameraShakeProfile shakeProfile;

    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        StartCoroutine(ShakeCamera());
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        transform.position += (speed * Time.fixedDeltaTime * transform.right);
    }

    // Phát hiện va chạm
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Enemy"))
        {
            HandleCollisionWithEnemy(_other);
        }
        else if(_other.CompareTag("Wall"))
        {
            HandleCollision(HitObjectSplashEffect);
            //Debug.Log("Cham vat can");
        }
    }
    private void HandleCollisionWithEnemy(Collider2D enemyCollider)
    {
        Enemy enemy = enemyCollider.GetComponent<Enemy>();
        if (enemy != null)
        {
            Vector2 hitDirection = (enemyCollider.transform.position - transform.position).normalized;
            enemy.EnemyGetsHit(damage, hitDirection, -hitForce);
        }

        HandleCollision(HitObjectSplashEffect);
    }
    private void HandleCollision(GameObject effect)
    {
        ActiveEffect(effect);
        Destroy(gameObject);
    }

    private void ActiveEffect(GameObject effect)
    {
        Quaternion rotation = transform.right.x > 0
            ? Quaternion.Euler(0, 180, 0)         
            : Quaternion.Euler(0, 0, 0);       

        Instantiate(effect, transform.position, rotation);
    }

    private IEnumerator ShakeCamera()
    {
        yield return new WaitForSeconds(0.2f);
        CameraManager.instance.CameraShakeFromProfile(shakeProfile, impulseSource);
    }
}
