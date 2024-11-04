using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FinalBoss : Enemy
{
    public static FinalBoss Instance;

    public GameObject LungeEffect,DiveEffect;
    public Transform introPosition;
    public Transform SideAttackTransform; //the middle of the side attack area
    public float SideAttackArea; //how large the area of side attack is

    public Transform UpAttackTransform; //the middle of the up attack area
    public float UpAttackArea; //how large the area of side attack is

    public Transform DownAttackTransform; //the middle of the down attack area
    public float DownAttackArea; //how large the area of down attack is

    public float attackRange;
    public float attackTimer;


    [Header("Audio")]
    public AudioClip Slash3Sound, Parrysound, Intro, DiveSound, DashSound, bounceSound, barrageOutbreakSound;


    //Attack
    [HideInInspector] public bool facingRight;
    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;

    //Lunge 
    [HideInInspector] public bool damagedPlayer = false;
    //Parry 
    [HideInInspector] public bool parrying;
    //Dive 
    [HideInInspector] public Vector2 moveToPosition;
    [HideInInspector] public bool diveAttack;
    public GameObject divingCollider;
    public GameObject pillar;
    //Barrage
    [HideInInspector] public bool barrageAttack;
    public GameObject barrageFireball,Barrage_Shoot_pos, BarrageEffect;
    //OutBreak
    [HideInInspector] public bool outbreakAttack;
    public GameObject outBreakEffect;
    //Bounds
    [HideInInspector] public bool bounceAttack;
    [HideInInspector] public float rotationDirectionToTarget;
    [HideInInspector] public int bounceCount;
    //Death
    public GameObject DeathEffect;

    [Header("Ground Check Settings:")]
    public Transform wallCheckPoint; //point at which wall check happens
    public Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround,WhatisWall; //sets the ground layer

   

    int hitCounter;
    bool stunned, canStun;
    public bool alive;
    bool attackable;

    [HideInInspector] public float runSpeed;

    public bool lungePerformed = false;
    public GameObject BackGroundChain1, BackGroundChain2, BackGroundChain3, BackGroundChain4,BaseChain1,BaseChain2,NormalTheme, BossTheme, BreakChainEffect;
    public GameObject CanvasHealthBar;

    public bool isGravity = false;
    private bool isActivated = false;
    public BoxCollider2D box;

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
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.FinalBoss_Stage1);
        alive = true;
        box = GetComponent<BoxCollider2D>();
        box.enabled = false;
        attackable = true;
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TouchedWall()
    {
        return Physics2D.OverlapCircle(wallCheckPoint.position, 0.5f, WhatisWall);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireSphere(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireSphere(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    protected override void Update()
    {
        ActiveBoss();
        if (isGravity)
        {
            enemyRb.gravityScale = 6f;
        }
        base.Update();
        if (health <= 0 && alive)
        {
            Death();
        }
        if (!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }
        if (stunned)
        {
            enemyRb.velocity = Vector2.zero;
        }
        CanvasHealth();
    }

    public void Flip()
    {
        if (PlayerController.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            //transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            Vector3 rotator = new Vector3(transform.rotation.x, 180, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = false;
        }
        else
        {
            //transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            Vector3 rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = true;
        }
    }

    protected override void UpdateEnemyStates()
    {
        if (PlayerController.Instance != null)
        {
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.FinalBoss_Stage1:
                    canStun = true;
                    attackTimer = 3;
                    runSpeed = speed;
                    break;

                case EnemyStates.FinalBoss_Stage2:
                    canStun = true;
                    attackTimer = 2;
                    break;

                case EnemyStates.FinalBoss_Stage3:
                    canStun = false;
                    attackTimer = 4;
                    break;

                case EnemyStates.FinalBoss_Stage4:
                    canStun = false;
                    attackTimer = 3;
                    runSpeed = speed / 2;
                    break;
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D _other) { }



    // Update is called once per frame

    public void ActiveBoss()
    {
        if (isActivated) return; // Nếu boss đã được kích hoạt, không làm gì cả

        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        if (_dist < detectDistance)
        {
            if (!IsShadeExist())
            {
                StartCoroutine(BossCutscene());
                isActivated = true; // Đặt cờ kích hoạt boss thành true
            }
        }
    }
    private bool IsShadeExist()
    {
        Shade existingShade = FindObjectOfType<Shade>();
        return existingShade != null;
    }

    void CanvasHealth()
    {
        if(!PlayerController.Instance.pState.alive)
        {
            CanvasHealthBar.SetActive(false);
        }
        /*else
        {
            CanvasHealthBar.SetActive(true);
        }*/
    }

    IEnumerator BossCutscene()
    {
        NormalTheme.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        box.enabled = true;
        PlayerController.Instance.pState.cutscenes = true;
        PlayerController.Instance.pState.Invincible = true;
        PlayerController.Instance.InputEnable = false;
        PlayerController.Instance.canMove = false;
        PlayerController.Instance.anim.SetTrigger("StopTrigger");
        PlayerController.Instance.anim.SetBool("Walking", false);
        PlayerController.Instance.rb.velocity = Vector3.zero;        // Di chuyển nhân vật đến vị trí chỉ định trong khi chờ đợi
        float moveDuration = 1f; // Thời gian di chuyển
        float elapsedTime = 0f;
        Vector3 startPosition = PlayerController.Instance.transform.position;
        Vector3 bossPosition = transform.position;
        Vector3 IntroPosition = introPosition.position;
        if (PlayerController.Instance.transform.position.x < IntroPosition.x)
        {
            // Quay mặt về phải
            PlayerController.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Quay mặt về trái
            PlayerController.Instance.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        while (elapsedTime < moveDuration)
        {
            PlayerController.Instance.Flip();
            PlayerController.Instance.transform.position = Vector3.MoveTowards(startPosition, introPosition.position, (elapsedTime / moveDuration) * Vector3.Distance(startPosition, introPosition.position));
            PlayerController.Instance.anim.SetBool("Walking", true);
            elapsedTime += Time.deltaTime;
            yield return null; // Chờ đợi khung hình tiếp theo
        }
        PlayerController.Instance.anim.SetTrigger("StopTrigger");
        PlayerController.Instance.anim.SetBool("Walking", false);
        if (PlayerController.Instance.transform.position.x < bossPosition.x)
        {
            // Quay mặt về phải
            PlayerController.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Quay mặt về trái
            PlayerController.Instance.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        yield return new WaitForSeconds(1.6f);
        _animator.SetBool("Intro", true);
        CanvasHealthBar.SetActive(true);
        BossTheme.SetActive(true);

        yield return new WaitForSeconds(9);
        PlayerController.Instance.pState.Invincible = false;
        PlayerController.Instance.InputEnable = true;
        PlayerController.Instance.canMove = true;
        PlayerController.Instance.pState.cutscenes = false;
    }
    public bool ChangeGravity()
    {
        return isGravity = true;
    }
    public void BreakChainEffectActive()
    {
        StartCoroutine(BreakChain());
    }
    
    IEnumerator BreakChain()
    {
        BreakChainEffect.SetActive(true);
        yield return new WaitForSeconds(1.4f);
        BreakChainEffect.SetActive(false);
    }

    public void DestroyBackgroundChains()
    {
        Destroy(BackGroundChain1);
        Destroy(BackGroundChain2);
        Destroy(BackGroundChain3);
        Destroy(BackGroundChain4);
        Destroy(BaseChain1);
        Destroy(BaseChain2);
    }
    public void AttackHandler()
    {
        float lungeRange = attackRange;
        float playerDistance = Vector2.Distance(PlayerController.Instance.transform.position, enemyRb.position);
        float xDifference = PlayerController.Instance.transform.position.x - transform.position.x;
        float yDifference = PlayerController.Instance.transform.position.y - transform.position.y;
        if (currentEnemyState == EnemyStates.FinalBoss_Stage1)
        {
            if (!attacking)
            {
                if (playerDistance <= attackRange)
                {
                    if (Mathf.Abs(xDifference) > Mathf.Abs(yDifference))
                    {
                        StartCoroutine(TripleSlash());
                    }
                    else if (yDifference > 0)
                    {
                        StartCoroutine(UpSlash());
                    }
                    else if (yDifference < 0)
                    {
                        StartCoroutine(DownSlash());
                    }
                }
                else if (playerDistance > lungeRange && !parrying)
                {
                    StartCoroutine(Lunge());
                }
            }
        }
        if (currentEnemyState == EnemyStates.FinalBoss_Stage2)
        {
            if (!attacking)
            {
                if (playerDistance <= attackRange)
                {
                    if (Mathf.Abs(xDifference) > Mathf.Abs(yDifference))
                    {
                        StartCoroutine(TripleSlash());
                    }
                    else if (yDifference > 0)
                    {
                        StartCoroutine(UpSlash());
                    }
                    else if (yDifference < 0)
                    {
                        StartCoroutine(DownSlash());
                    }
                }
                else if (playerDistance > lungeRange && !parrying)
                {
                    int _attackChosen = Random.Range(1, 3);
                    if (_attackChosen == 1)
                    {
                        StartCoroutine(Lunge());
                    }
                    if (_attackChosen == 2)
                    {
                        if (!diveAttack)
                        {
                            DiveAttackJump();
                        }
                    }
                    if (_attackChosen == 3)
                    {
                        BarrageBendDown();
                    }
                }
            }
        }
        if (currentEnemyState == EnemyStates.FinalBoss_Stage3)
        {
            if (!attacking)
            {
                int _attackChosen = Random.Range(1, 4);
                if (_attackChosen == 1)
                {
                    OutbreakBendDown();
                }
                if (_attackChosen == 2)
                {
                    if (!diveAttack)
                    {
                        DiveAttackJump();
                    }
                }
                if (_attackChosen == 3)
                {
                    BarrageBendDown();
                }
                if (_attackChosen == 4)
                {
                    BounceAttack();
                }
            }
        }
        if (currentEnemyState == EnemyStates.FinalBoss_Stage4)
        {
            if (!attacking)
            {
                if (playerDistance <= attackRange)
                {
                    if (Mathf.Abs(xDifference) > Mathf.Abs(yDifference))
                    {
                        StartCoroutine(TripleSlash());
                    }
                    else if (yDifference > 0)
                    {
                        StartCoroutine(UpSlash());
                    }
                    else if (yDifference < 0)
                    {
                        StartCoroutine(DownSlash());
                    }
                }
                else if (playerDistance > lungeRange)
                {
                    BounceAttack();
                }
            }
        }
    }
    public void ResetAllAttacks()
    {
        Debug.Log("ResetAllAttacks called at: " + Time.time);
        attacking = false;
        diveAttack = false;
        barrageAttack = false;
        outbreakAttack = false;
        bounceAttack = false;
        StopCoroutine(TripleSlash());
        StopCoroutine(UpSlash());
        StopCoroutine(DownSlash());
        StopCoroutine(Lunge());
        StopCoroutine(Parry());
        StopCoroutine(Slash());
    }

    IEnumerator Lunge()
    {
        Flip();
        yield return new WaitForSeconds(0.5f);
        attacking = true;
        attackable = false;
        _animator.SetBool("Lunge", true);
        Instantiate(LungeEffect, transform);
        PlayDashSound();
        yield return new WaitForSeconds(1f);
        _animator.SetBool("Lunge", false);
        damagedPlayer = false;
        Flip();
        Vector3 moveBackPosition = transform.position + new Vector3(facingRight ? -7f : 7f, 0, 0);
        float moveBackDuration = 1f; // Thời gian để lùi lại
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < moveBackDuration)
        {
            if (TouchedWall())
            {
                break;
            }
            transform.position = Vector3.Lerp(startPosition, moveBackPosition, elapsedTime / moveBackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (!TouchedWall())
        {
            transform.position = moveBackPosition;
        }
        attackable = true;
        yield return new WaitForSeconds(2f);
        ResetAllAttacks();
    }

    IEnumerator UpSlash()
    {
        Flip();
        attacking = true;
        attackable = false;
        enemyRb.velocity = Vector2.zero;
        _animator.SetTrigger("UpSlash");
        yield return new WaitForSeconds(1f);
        attackable = true;
        _animator.ResetTrigger("UpSlash");
        ResetAllAttacks();
    }
    IEnumerator DownSlash()
    {
        Flip();
        attacking = true;
        enemyRb.velocity = Vector2.zero;
        attackable = false;
        _animator.SetTrigger("DownSlash");
        yield return new WaitForSeconds(1f);
        attackable = true;
        _animator.ResetTrigger("DownSlash");
        ResetAllAttacks();
    }

    IEnumerator TripleSlash()
    {
        Flip();
        attacking = true;
        enemyRb.velocity = Vector2.zero;

        attackable = false;
        _animator.SetTrigger("Slash");
        yield return new WaitForSeconds(0.3f);
        //SlashAngle();
        attackable = true;
        yield return new WaitForSeconds(0.1f);
        _animator.ResetTrigger("Slash");
        Flip();
        attackable = false;
        _animator.SetTrigger("Slash 2");
        yield return new WaitForSeconds(0.3f);
        //SlashAngle();
        attackable = true;
        yield return new WaitForSeconds(0.2f);
        _animator.ResetTrigger("Slash 2");
        Flip();
        attackable = false;
        _animator.SetTrigger("Slash 3");
        yield return new WaitForSeconds(0.3f);
        //SlashAngle();
        attackable = true;
        yield return new WaitForSeconds(0.3f);
        _animator.ResetTrigger("Slash 3");

        ResetAllAttacks();
    }
    IEnumerator Slash()
    {
        Flip();
        attacking = true;
        enemyRb.velocity = Vector2.zero;

        _animator.SetTrigger("Parried");
        yield return new WaitForSeconds(0.1f);
        _animator.ResetTrigger("Parried");
        _animator.SetTrigger("Slash 3");
        yield return new WaitForSeconds(0.3f);
        _animator.ResetTrigger("Slash 3");
        //_animator.SetTrigger("Idle");


        ResetAllAttacks();
    }
    IEnumerator Parry()
    {
        _animator.SetBool("Parry", true);
        yield return new WaitForSeconds(1f);
        _animator.SetBool("Parry", false);
    }

    public void StartParry()
    {
        Debug.Log("Starting Parry coroutine. parrying = true");
        attacking = true;
        enemyRb.velocity = Vector2.zero;
    }

    public void StopParry()
    {
        parrying = false;
        Debug.Log("Ending Parry coroutine. parrying = false");
        ResetAllAttacks();
    }

    public override void AttackPlayer()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
    public override void Turn() {}
    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (attackable && alive)
        {
            if (!stunned)
            {
                if (!parrying)
                {
                    if (canStun)
                    {
                        hitCounter++;
                        if (hitCounter >= 12)
                        {
                            ResetAllAttacks();
                            StartCoroutine(Stunned());
                        }
                    }
                    ResetAllAttacks();
                    base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);

                    if (currentEnemyState != EnemyStates.FinalBoss_Stage4 && currentEnemyState != EnemyStates.FinalBoss_Stage3 && health >0)
                    {
                        int randomParry = Random.Range(1, 4);
                        if (randomParry >= 3)
                        {
                            ResetAllAttacks();
                            StartCoroutine(Parry());
                        }
                    }

                }
                else
                {
                    Debug.Log("Parrying. Switching to Slash coroutine.");
                    StopCoroutine(Parry());
                    parrying = false;
                    ResetAllAttacks();
                    StartCoroutine(Slash());
                }
            }
            else
            {
                StopCoroutine(Stunned());
                _animator.SetBool("Stunned", false);
                stunned = false;
            }
        }
        if (health > 20)
        {
            Debug.Log("Stage 1");
            ChangeState(EnemyStates.FinalBoss_Stage1);
        }
        if (health <= 20 && health > 14)
        {
            Debug.Log("Stage 2");
            ChangeState(EnemyStates.FinalBoss_Stage2);
        }
        if (health <= 14 && health > 7)
        {
            Debug.Log("Stage 3");
            ChangeState(EnemyStates.FinalBoss_Stage3);
        }
        if (health <= 7 && health > 0)
        {
            Debug.Log("Stage 4");
            ChangeState(EnemyStates.FinalBoss_Stage4);
        }
        if (health <= 0 && alive)
        {
            Death();
        }
    }
    void DiveAttackJump()
    {
        Debug.Log("Calling DiveAttackJump at: " + Time.time);
        attacking = true;
        attackable = false;
        moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, enemyRb.position.y + 5);
        diveAttack = true;
        _animator.SetBool("Jump", true);
    }

    public void Dive()
    {
        _animator.SetBool("Dive", true);
        _animator.SetBool("Jump", false);

    }
    private void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.GetComponent<PlayerController>() != null && (diveAttack || bounceAttack))
        {
            _other.GetComponent<PlayerController>().TakeDamage(damage * 2);
            PlayerController.Instance.pState.recoilingX = true;
        }
    }
    public void DiveActive()
    {
        DiveEffect.SetActive(true);
        attackable = true;
    }

    public void DiveHitGround()
    {
        DiveEffect.SetActive(false);
    }
    public void DivingPillars()
    {
        Vector2 _impactPoint = groundCheckPoint.position;
        float _spawnDistance = 5;

        for (int i = 0; i < 5; i++)
        {
            Vector2 _pillarSpawnPointRight = _impactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnPointLeft = _impactPoint - new Vector2(_spawnDistance, 0);
            Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, 0));
            Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, 0));

            _spawnDistance += 5;
        }
        ResetAllAttacks();
    }
    void BarrageBendDown()
    {
        attacking = true;
        enemyRb.velocity = Vector2.zero;
        Flip();
        barrageAttack = true;
        attackable = false;
        _animator.SetTrigger("BendDown");
    }

    public IEnumerator Barrage()
    {
        enemyRb.velocity = Vector2.zero;
        GameObject Effect = Instantiate(BarrageEffect, transform);
        float _currentAngle = 20f;
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("launch");
            GameObject _projectile = Instantiate(barrageFireball, Barrage_Shoot_pos.transform.position, Quaternion.Euler(0, 0, _currentAngle));

            if (facingRight)
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle);
            }
            else
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
            }

            _currentAngle += 3f;
            PlayBarrageSound();
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(1f);
        attackable = true;
        _animator.SetBool("Barrage", false);
        ResetAllAttacks();
    }
    void OutbreakBendDown()
    {
        attacking = true;
        enemyRb.velocity = Vector2.zero;
        attackable = false;
        moveToPosition = new Vector2(transform.position.x, enemyRb.position.y + 3);
        outbreakAttack = true;
        _animator.SetTrigger("BendDown");
    }
    public IEnumerator Outbreak()
    {
        yield return new WaitForSeconds(1f);
        _animator.SetBool("Cast", true);

        enemyRb.velocity = Vector2.zero;
        GameObject Effect = Instantiate(outBreakEffect, transform);
        for (int i = 0; i < 30; i++)
        {
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(90, 180)));
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 90))); 
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(190, 350)));
            yield return new WaitForSeconds(0.2f);
            PlayBarrageSound();
        }
        yield return new WaitForSeconds(0.1f);
        box.enabled = true;
        enemyRb.constraints = RigidbodyConstraints2D.None;
        enemyRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        enemyRb.velocity = new Vector2(enemyRb.velocity.x, -3);
        attackable = true;  
        yield return new WaitForSeconds(0.1f);
        _animator.SetBool("Cast", false);
        ResetAllAttacks();
    }
    void BounceAttack()
    {
        attacking = true;
        bounceCount = Random.Range(2, 5);
        BounceBendDown();
    }
    int _bounces = 0;
    public void CheckBounce()
    {
        if (_bounces < bounceCount - 1)
        {
            _bounces++;
            BounceBendDown();
        }
        else
        {
            _bounces = 0;
            _animator.Play("FinalBoss_Run");
        }
    }
    public void BounceBendDown()
    {
        attackable = false;
        enemyRb.velocity = Vector2.zero;
        moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, enemyRb.position.y + 5);
        bounceAttack = true;
        _animator.SetTrigger("BendDown");
    }

    public void CalculateTargetAngle()
    {
        Vector3 _directionToTarget = (PlayerController.Instance.transform.position - transform.position).normalized;

        float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
        rotationDirectionToTarget = _angleOfTarget;
    }
    public IEnumerator Stunned()
    {
        stunned = true;
        hitCounter = 0;
        _animator.SetBool("Stunned", true);
        yield return new WaitForSeconds(6f);
        _animator.SetBool("Stunned", false);
        stunned = false;
    }
    void Death()
    {
        ResetAllAttacks();
        alive = false;
        enemyRb.velocity = new Vector2(enemyRb.velocity.x, -7);
        _animator.SetTrigger("Die");
    }

    public IEnumerator DestroyAfterDeath()
    {
        BossTheme.SetActive(false);
        CanvasHealthBar.SetActive(false);
        Instantiate(DeathEffect,transform.position, Quaternion.identity);
        gameObject.layer = LayerMask.NameToLayer("Decoration");
        GlobalController.instance.playerScore += scoreValue;
        SaveData.Instance.SavePlayerData();
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

    public void PlayDeathSound()
    {
        audioSource.volume = 0.8f;
        audioSource.PlayOneShot(deathSound);
    }
    public void PlayIntroSound()
    {
        audioSource.volume = 0.8f;
        audioSource.PlayOneShot(Intro);
    }
    public void PlaySlash3Sound()
    {
        audioSource.volume = 0.8f;
        audioSource.PlayOneShot(Slash3Sound);
    }
    public void PlaySlashSound()
    {
        audioSource.volume = 0.8f;
        audioSource.PlayOneShot(Hitsound);
    }
    public void PlayParrySound()
    {
        audioSource.volume = 0.3f;
        audioSource.PlayOneShot(Parrysound);
    }
    public void PlayDiveSound()
    {
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(DiveSound);
    }
    public void PlayDashSound()
    {
        audioSource.volume = 0.6f;
        audioSource.PlayOneShot(DashSound);
    }
    public void PlayBounceSound()
    {
        audioSource.volume = 0.6f;
        audioSource.PlayOneShot(bounceSound);
    }
    public void PlayBarrageSound()
    {
        audioSource.volume = 0.3f;
        audioSource.PlayOneShot(barrageOutbreakSound);
    }

    public float currentHealth
    {
        get { return Health; }
        set { }
    }
}
