using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemy : Enemy
{
    [Header("Charger Settings:")]
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float HurtDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;


    float timer;
    private bool isFacingPlayer = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Charger_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Charger_Idle);
        }
    }
    protected override void UpdateEnemyStates()
    {
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Charger_Idle:
                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                 || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }
                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 7);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    isFacingPlayer = true;
                    ChangeState(EnemyStates.Charger_Detect);
                }
                if (transform.localScale.x > 0)
                {
                    enemyRb.velocity = new Vector2(speed, enemyRb.velocity.y);
                }
                else
                {
                    enemyRb.velocity = new Vector2(-speed, enemyRb.velocity.y);
                }
                break;
            case EnemyStates.Charger_Detect:
                if (!PlayerController.Instance.pState.Invincible)
                {
                    enemyRb.velocity = new Vector2(0, jumpForce);

                    ChangeState(EnemyStates.Charger_Charge);
                }
                else
                {
                    enemyRb.velocity = new Vector2(0, 0);
                    ChangeState(EnemyStates.Charger_Idle);
                }
                break;

            case EnemyStates.Charger_Charge:
                timer += Time.deltaTime;
                if (timer < chargeDuration && isFacingPlayer)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        if (transform.localScale.x > 0)
                        {
                            enemyRb.velocity = new Vector2(speed * chargeSpeedMultiplier, enemyRb.velocity.y);
                        }
                        else
                        {
                            enemyRb.velocity = new Vector2(-speed * chargeSpeedMultiplier, enemyRb.velocity.y);
                        }
                    }
                    else
                    {
                        enemyRb.velocity = new Vector2(0, enemyRb.velocity.y);
                    }
                }
                else
                {
                    isFacingPlayer = false;
                    timer = 0;
                    ChangeState(EnemyStates.Charger_Idle);
                }
                break;
            case EnemyStates.Charger_Hurt:
                    StartCoroutine(HurtDelay());
                    ChangeState(EnemyStates.Charger_Idle);
                break;
            case EnemyStates.Charger_Death:

                break;
        }
    }

    IEnumerator HurtDelay()
    {
        yield return new WaitForSeconds(recoilLength);
    }
    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Charger_Hurt);
        }
        else
        {
            ChangeState(EnemyStates.Charger_Death);
        }
        if(!isFacingPlayer)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            isFacingPlayer = true;
        }
    }
    protected override void ChangeCurrentAnimation()
    {
        _animator.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Charger_Idle);

        _animator.SetBool("isDetect", GetCurrentEnemyState == EnemyStates.Charger_Detect);

        _animator.SetBool("isAttack", GetCurrentEnemyState == EnemyStates.Charger_Charge);
        _animator.SetBool("isHurt", GetCurrentEnemyState == EnemyStates.Charger_Hurt);
        if (GetCurrentEnemyState == EnemyStates.Charger_Death)
        {
            Vector2 newVelocity;
            newVelocity.x = 0;
            newVelocity.y = 0;
            enemyRb.velocity = newVelocity;
            _animator.SetTrigger("isDeath");
            audioSource.PlayOneShot(deathSound);
            gameObject.layer = LayerMask.NameToLayer("Decoration");
            StartCoroutine(fadeCoroutine());
        }
    }
    private IEnumerator fadeCoroutine()
    {
        GlobalController.instance.playerScore += scoreValue;
        GlobalController.instance.EnemyKilled();
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
        ChargeEnemyPool.Instance.ReturnEnemyToPool(gameObject);
    }
    public override void TrapHit(float _damageDone)
    {
        base.TrapHit(_damageDone);
        ChangeState(EnemyStates.Charger_Death);
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
        isFacingPlayer = false;
    }
}

