using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PatrolEnemy : Enemy
{
    [Header("Crawler Settings:")]
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float RunSpeed;

    float timer;


    private bool _isMovable = true;


    protected override void Start()
    {
        base.Start();
    }
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        if (health > 0)
        {
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Patrol_Idle:
                    _animator.SetBool("isChasing", false);
                    _animator.ResetTrigger("isAttack");
                    if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                        || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                    {
                        ChangeState(EnemyStates.Patrol_Flip);
                    }
                    if (transform.localScale.x > 0)
                    {
                        enemyRb.velocity = new Vector2(speed, enemyRb.velocity.y);
                    }
                    else
                    {
                        enemyRb.velocity = new Vector2(-speed, enemyRb.velocity.y);
                    }
                    if (_dist < detectDistance && Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                        || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                    {
                        ChangeState(EnemyStates.Patrol_Chase);
                    }
                    break;
                case EnemyStates.Patrol_Flip:
                    timer += Time.deltaTime;

                    if (timer > flipWaitTime)
                    {
                        timer = 0;
                        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                        ChangeState(EnemyStates.Patrol_Idle);
                    }
                    if (_dist < detectDistance && Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                        || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                    {
                        ChangeState(EnemyStates.Patrol_Chase);
                    }
                    break;
                case EnemyStates.Patrol_Chase:
                    if (!PlayerController.Instance.pState.Invincible)
                    {
                        if (!isRecoiling && _isMovable)
                        {
                            _animator.SetBool("isChasing", true);
                            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), RunSpeed * Time.deltaTime);
                        }
                        Flip();
                    }
                    else
                    {
                        Flip();
                        {
                            enemyRb.velocity = new Vector2(0, 0);
                        }
                    }
                    if (_dist > detectDistance)
                    {
                        ChangeState(EnemyStates.Patrol_Idle);
                    }
                    break;
            }
        }
    }
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Patrol_Idle);
        }
        _playerEnemyDistance = _playerTransform.position.x - _transform.position.x;
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
    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            audioSource.PlayOneShot(hurtSound);
            GameObject _orangeBlood = Instantiate(Blood, transform.position, Quaternion.identity);
            Destroy(_orangeBlood, 2.5f);
            enemyRb.velocity = _hitForce * recoilFactor * _hitDirection;
            isRecoiling = true;
            _isMovable = false;
            StartCoroutine(hurtCoroutine());
        }
    }

    public override void TrapHit(float _damageDone)
    {
        base.TrapHit(_damageDone);
        StartCoroutine(hurtCoroutine());
    }

    private IEnumerator hurtCoroutine()
    {
        if (health <= 0)
        {
            _isMovable = false;
            die();
            
            yield return null;
        }
        else
        {
            _animator.SetTrigger("isHurt");
            yield return new WaitForSeconds(recoilLength);
            _animator.ResetTrigger("isHurt");
            _isMovable = true;
        }
    }


    protected void die()
    {
        audioSource.PlayOneShot(deathSound);
        _animator.SetTrigger("isDead");

        Vector2 newVelocity;
        newVelocity.x = 0;
        newVelocity.y = 0;
        enemyRb.velocity = newVelocity;

        gameObject.layer = LayerMask.NameToLayer("Decoration");
        StartCoroutine(fadeCoroutine());
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
        PatrolEnemyPool.Instance.ReturnEnemyToPool(gameObject);
    }

    public override void AttackPlayer()
    {
        _animator.SetTrigger("isAttack");
        audioSource.PlayOneShot(Hitsound);
        PlayerController.Instance.TakeDamage(damage);
    }
    public override void Turn()
    {
        ChangeState(EnemyStates.Patrol_Flip);
    }
    public override void ResetStats()
    {
        base.ResetStats();
        _isMovable = true;
        timer = 0;
    }
}
