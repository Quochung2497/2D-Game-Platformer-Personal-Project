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
    private Collider2D enemyCollider;
    private ContactFilter2D playerDetectionFilter;
    private readonly RaycastHit2D[] playerDetectionHits = new RaycastHit2D[1];
    private int attackableLayerIndex;
    private const float groundProbeOffset = 0.05f;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        enemyCollider = GetComponent<Collider2D>();
        attackableLayerIndex = LayerMask.NameToLayer("Attackable");
        playerDetectionFilter.SetLayerMask(Physics2D.DefaultRaycastLayers);
        playerDetectionFilter.useTriggers = false;
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
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Charger_Idle:
                Bounds bounds = enemyCollider.bounds;
                bool canPatrol = true;
                bool isGrounded = Physics2D.Raycast(bounds.center, Vector2.down, ledgeCheckY, whatIsGround);
                if (isGrounded && (!HasGroundAhead(bounds, direction) || HasWallAhead(bounds, direction)))
                {
                    Vector2 reverseDirection = -direction;
                    if (HasGroundAhead(bounds, reverseDirection) && !HasWallAhead(bounds, reverseDirection))
                    {
                        Turn();
                        direction = reverseDirection;
                    }
                    else
                    {
                        canPatrol = false;
                    }
                }

                if (DetectPlayer(direction))
                {
                    isFacingPlayer = true;
                    ChangeState(EnemyStates.Charger_Detect);
                }

                enemyRb.linearVelocity = new Vector2(canPatrol ? direction.x * speed : 0f, enemyRb.linearVelocity.y);
                break;
            case EnemyStates.Charger_Detect:
                if (!PlayerController.Instance.pState.Invincible)
                {
                    enemyRb.linearVelocity = new Vector2(0, jumpForce);

                    ChangeState(EnemyStates.Charger_Charge);
                }
                else
                {
                    enemyRb.linearVelocity = new Vector2(0, 0);
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
                            enemyRb.linearVelocity = new Vector2(speed * chargeSpeedMultiplier, enemyRb.linearVelocity.y);
                        }
                        else
                        {
                            enemyRb.linearVelocity = new Vector2(-speed * chargeSpeedMultiplier, enemyRb.linearVelocity.y);
                        }
                    }
                    else
                    {
                        enemyRb.linearVelocity = new Vector2(0, enemyRb.linearVelocity.y);
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

    private bool HasGroundAhead(Bounds bounds, Vector2 direction)
    {
        Vector2 origin = (Vector2)bounds.center + direction * (bounds.extents.x + groundProbeOffset);
        return Physics2D.Raycast(origin, Vector2.down, ledgeCheckY, whatIsGround);
    }

    private bool HasWallAhead(Bounds bounds, Vector2 direction)
    {
        return Physics2D.Raycast(bounds.center, direction, bounds.extents.x + groundProbeOffset, whatIsGround);
    }

    private bool DetectPlayer(Vector2 direction)
    {
        Vector2 origin = (Vector2)transform.position + direction * ledgeCheckX;
        int hitCount = Physics2D.Raycast(origin, direction, playerDetectionFilter, playerDetectionHits, ledgeCheckX * 7);
        return hitCount > 0 && playerDetectionHits[0].collider.CompareTag("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (health <= 0 || collision.collider.gameObject.layer != attackableLayerIndex) return;

        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector2.Dot(collision.GetContact(i).normal, direction) < -0.5f)
            {
                Turn();
                return;
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != attackableLayerIndex)
        {
            base.OnCollisionStay2D(collision);
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
            enemyRb.linearVelocity = newVelocity;
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

