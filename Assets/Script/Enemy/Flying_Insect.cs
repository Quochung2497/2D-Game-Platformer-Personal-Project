using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying_Insect : Enemy
{
    [SerializeField] private float stunDuration;

    float timer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Insect_Idle);
    }
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        if (health > 0)
        {
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Insect_Idle:
                    enemyRb.velocity = new Vector2(0, 0);
                    if (_dist < detectDistance)
                    {
                        ChangeState(EnemyStates.Insect_Chase);
                    }
                    break;

                case EnemyStates.Insect_Chase:
                    if (!PlayerController.Instance.pState.Invincible)
                    {
                        enemyRb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                        Flip();
                    }
                    else
                    {
                        Flip();
                        enemyRb.velocity = new Vector2(0, 0);
                    }
                    if (_dist > detectDistance)
                    {
                        ChangeState(EnemyStates.Insect_Idle);
                    }
                    break;
                case EnemyStates.Insect_Stunned:
                    timer += Time.deltaTime;

                    if (timer > stunDuration)
                    {
                        ChangeState(EnemyStates.Insect_Idle);
                        timer = 0;
                    }
                    break;
                case EnemyStates.Insect_Death:

                    break;
            }
        }
    }
    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Insect_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Insect_Death);
        }
    }
    public override void TrapHit(float _damageDone)
    {
        base.TrapHit(_damageDone);
        ChangeState(EnemyStates.Insect_Death);
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
        FlyingEnemyPool.Instance.ReturnEnemyToPool(gameObject);
    }

    protected override void ChangeCurrentAnimation()
    {
        _animator.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Insect_Idle);

        _animator.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Insect_Chase);

        _animator.SetBool("Stun", GetCurrentEnemyState == EnemyStates.Insect_Stunned);
        if (GetCurrentEnemyState == EnemyStates.Insect_Death)
        {
            audioSource.PlayOneShot(deathSound);
            _animator.SetTrigger("Death");
            enemyRb.gravityScale = 6f;
            gameObject.layer = LayerMask.NameToLayer("Decoration");
            StartCoroutine(fadeCoroutine());
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
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Insect_Idle);
        }
        _playerEnemyDistance = _playerTransform.position.x - _transform.position.x;
    }

    public override void AttackPlayer()
    {
        audioSource.PlayOneShot(Hitsound);
        PlayerController.Instance.TakeDamage(damage);
    }
    public override void Turn()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }
    public override void ResetStats()
    {
        base.ResetStats();
        timer = 0;
    }
}
