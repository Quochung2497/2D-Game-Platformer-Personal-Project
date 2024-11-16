using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerController : Enemy
{
    public float shootInterval;
    public GameObject projectilePrefab;

    private bool _isShootable = true;
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Gunner_Idle);
    }

    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Gunner_Idle);
        }
        _playerEnemyDistance = _playerTransform.position.x - _transform.position.x;
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        if (health > 0)
        {
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Gunner_Idle:
                    enemyRb.velocity = new Vector2(0, 0);
                    if (_dist < detectDistance)
                    {
                        ChangeState(EnemyStates.Gunner_Attack);
                    }
                    break;

                case EnemyStates.Gunner_Attack:
                    if (!PlayerController.Instance.pState.Invincible)
                    {
                        Flip();
                        
                        shootPlayer();
                    }
                    if (_dist > detectDistance)
                    {
                        ChangeState(EnemyStates.Gunner_Idle);
                    }
                    break;
                case EnemyStates.Gunner_Death:

                    break;
            }
        }
    }

    private void shootPlayer()
    {
        if (_isShootable)
        {
            _isShootable = false;
            Vector2 direction = _playerTransform.position - _transform.position;
            StartCoroutine(shootPlayerCoroutine(direction, shootInterval));
        }
    }

    private IEnumerator shootPlayerCoroutine(Vector2 direction, float shootInterval)
    {

        yield return new WaitForSeconds(0.2f);
        GameObject projectileObj = Instantiate(projectilePrefab, _transform.position, _transform.rotation);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.direction = direction;
        projectile.trigger();
        yield return new WaitForSeconds(shootInterval);
        if(!_isShootable)
        {
            _isShootable=true;
        }
    }
    void Flip()
    {
        float playerPosX = PlayerController.Instance.transform.position.x;
        float enemyPosX = transform.position.x;

        if (playerPosX < enemyPosX)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            enemyRb.velocity = new Vector2(-speed, enemyRb.velocity.y);
        }
        else if (playerPosX > enemyPosX)
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            enemyRb.velocity = new Vector2(speed, enemyRb.velocity.y);
        }
    }
    private IEnumerator fadeCoroutine()
    {
        GlobalController.instance.playerScore += scoreValue;
        while (destroyDelay > 0)
        {
            destroyDelay -= Time.deltaTime;

            if (_spriteRenderer.color.a > 0)
            {
                Color newColor = _spriteRenderer.color;
                newColor.a -= Time.deltaTime / destroyDelay;
                _spriteRenderer.color = newColor;
                yield return null;
            }
        }
        ResetStats();
        GunnerEnemyPool.Instance.ReturnEnemyToPool(gameObject);
    }

    protected override void ChangeCurrentAnimation()
    {
        _animator.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Gunner_Idle);

        _animator.SetBool("Attack", GetCurrentEnemyState == EnemyStates.Gunner_Attack);

        if (GetCurrentEnemyState == EnemyStates.Gunner_Death)
        {
            audioSource.PlayOneShot(deathSound);
            _animator.SetTrigger("Death");
            enemyRb.gravityScale = 6f;
            gameObject.layer = LayerMask.NameToLayer("Decoration");
            StartCoroutine(fadeCoroutine());
        }
    }
    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);
        if(health <=0)
        {
            ChangeState(EnemyStates.Gunner_Death);
        }
    }
    public override void AttackPlayer()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
    public override void Turn()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }
    public override void TrapHit(float _damageDone)
    {
        base.TrapHit(_damageDone);
        ChangeState(EnemyStates.Gunner_Death);
    }
}
