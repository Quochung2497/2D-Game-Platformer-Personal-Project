using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shade : Enemy
{
    [SerializeField] private float stunDuration;
    [SerializeField] private GameObject HurtEffect;
    float timer;
    public static Shade Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        //Debug.Log("shade spawned");
        SaveData.Instance.SaveShadeData();
    }
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Shade_Idle);
    }
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        if (health > 0)
        {
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Shade_Idle:
                    enemyRb.velocity = new Vector2(0, 0);
                    if (_dist < detectDistance)
                    {
                        ChangeState(EnemyStates.Shade_Chase);
                    }
                    break;

                case EnemyStates.Shade_Chase:
                    if (!PlayerController.Instance.pState.Invincible)
                    {
                        enemyRb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                        Flip();
                    }
                    else
                    {
                        Flip();
                        enemyRb.velocity = new Vector2 (0, 0);
                    }
                    if (_dist > detectDistance)
                    {
                        ChangeState(EnemyStates.Shade_Idle);
                    }
                    break;
                case EnemyStates.Shade_Stunned:
                    timer += Time.deltaTime;

                    if (timer > stunDuration)
                    {
                        ChangeState(EnemyStates.Shade_Idle);
                        timer = 0;
                    }
                    break;
                case EnemyStates.Shade_Death:

                    break;
            }
        }
    }
    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            audioSource.PlayOneShot(hurtSound);
            Instantiate(HurtEffect, transform.position, Quaternion.identity);
            GameObject _orangeBlood = Instantiate(Blood, transform.position, Quaternion.identity);
            Destroy(_orangeBlood, 2.5f);
            enemyRb.velocity = _hitForce * recoilFactor * _hitDirection;
            isRecoiling = true;
        }
        if (health > 0)
        {
            ChangeState(EnemyStates.Shade_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Shade_Death);
        }
    }
    public override void TrapHit(float _damageDone)
    {
        base.TrapHit(0);
        ChangeState(EnemyStates.Shade_Death);
    }


    private IEnumerator fadeCoroutine()
    {

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
        Destroy(gameObject);
    }

    protected override void ChangeCurrentAnimation()
    {
        _animator.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Shade_Idle);

        _animator.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Shade_Chase);

        _animator.SetBool("Stun", GetCurrentEnemyState == EnemyStates.Shade_Stunned);
        if (GetCurrentEnemyState == EnemyStates.Shade_Death)
        {
            PlayerController.Instance.RestoreMana();
            SaveData.Instance.SavePlayerData();
            _animator.SetTrigger("Death");
            audioSource.PlayOneShot(deathSound);
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
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Shade_Idle);
        }
        _playerEnemyDistance = _playerTransform.position.x - _transform.position.x;
    }

    public override void AttackPlayer()
    {
        //_animator.SetTrigger("isAttack");
        PlayerController.Instance.TakeDamage(damage);
    }
    public override void Turn()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }
}
