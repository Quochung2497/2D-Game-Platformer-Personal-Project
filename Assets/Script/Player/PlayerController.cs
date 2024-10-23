using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{

    //Movement

    [Header("Horizontal Movement Settings: ")]
    [SerializeField] private float walkSpeed = 6;
    [Space(5)]

    [Header("Vertical Movement Settings: ")]
    [SerializeField] private float jumpForce = 25;
    private int JumpBufferCounter = 0;
    [SerializeField] private int JumpBufferFrames;
    private float CoyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    public int AirJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    [SerializeField] GameObject JumpEffect, doubleJumpEffect, landEffect;
    private float gravity;
    private int JumpEffectCreated = 0;
    [Space(5)]

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform FootCheckPoint;
    [SerializeField] private float FootCheckY = 0.2f;
    [SerializeField] private float FootCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround, WhatIsWater;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject StartdashEffect, dashEffect;
    public bool canDash = true, dashed;
    [Space(5)]

    [Header("Attack Settings")]
    //attack
    [SerializeField] private float AtkInterval;
    public Transform attackForwardPoint, UpAttackPoint, DownAttackPoint;
    public float SideAttackRange = 0.5f, UpAttackRange = 0.5f, DownAttackRange = 0.5f;
    private float timeSinceAttack, attackeffectdelay = 0.1f;
    public bool attack = false, attackable = true;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;
    //Combo
    private int comboStep = 0;
    [SerializeField] private float comboResetTime = 0.8f;

    //Receive damage
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil Settings:")]
    [SerializeField] private float recoilXSpeed;
    [SerializeField] private float recoilYSpeed;
    [SerializeField] private int recoilXSteps;
    [SerializeField] private int recoilYSteps;
    private int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    public int maxTotalHealth = 10;
    public int heartShards;
    [SerializeField] GameObject Blood, HurtEffect;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    float healTimer;
    [SerializeField] float timeToHeal;
    [SerializeField] GameObject HealedEffect, HealingEffect;
    [Space(5)]

    [Header("Mana Settings")]
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    public bool halfMana;
    [Space(5)]

    [Header("Spell Settings")]
    //spell stats
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    float timeSinceCast;
    [SerializeField] float spellDamage; //upspellexplosion and downspell
    [SerializeField] float downSpellForce; // desolate dive only
    [SerializeField] GameObject sideSpell, upSpell, downSpell, downSpellEffect;
    float castOrHealTimer;
    [Space(5)]
    [Header("Camera Stuff")]
    [SerializeField] private float playerFallSpeedThreshold = -10;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isWallSliding;
    bool isWallJumping;
    [Space(5)]

    [Header("Audio")]
    AudioManager audioManager;
    private bool landingSoundPlayed;

    [Header("VFX")]
    [HideInInspector] public PlayerParticles PlayerParticles;
    [Header("")]

    //unlocking 
    public bool unlockedWallJump;
    public bool unlockedDash;
    public bool unlockedVarJump;
    public bool unlockedHeal;
    public bool unlockedCastSpell;
    //References
    public GameObject EndingEffect;
    [HideInInspector] public Rigidbody2D rb;
    private float xAxis, yAxis;
    bool openMap;
    public Animator anim;
    private SpriteRenderer sr;
    public float fallThreshold = -12f;
    public bool canMove = true, InputEnable = true;
    public bool isFacingRight = true;
    [SerializeField] private float _maxFallSpeed = -50f;
    public float fallSpeedIncrease = -2.0f;
    private float currentFallSpeed;
    public static PlayerController Instance;
    [HideInInspector] public PlayerStateList pState;
    private bool canFlash = true;


    void Start()
    {
        CharacterStart();
    }

    void Update()
    {
        HandleController();        
    }
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
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void FixedUpdate()
    {
        if (pState.dashing || pState.healing || pState.cutscenes) return;
        Recoil();
    }
    #region Start-Update
    private void CharacterStart()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentFallSpeed = 0.0f;
        pState = GetComponent<PlayerStateList>();
        sr = GetComponent<SpriteRenderer>();
        PlayerParticles = GetComponentInChildren<PlayerParticles>();
        gravity = rb.gravityScale;
        Mana = mana;
        manaStorage.fillAmount = Mana;
        Health = maxHealth;
        SaveData.Instance.LoadPlayerData();
        if (halfMana)
        {
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
        }
        else
        {
            UIManager.Instance.SwitchMana(UIManager.ManaState.FullMana);
        }
        onHealthChangedCallback.Invoke();
        if (Health <= 0)
        {
            pState.alive = false;
            GlobalController.instance.RespawnPlayer();
        }
    }

    private void HandleController()
    {
        if (PauseMenuUI.Instance.GameIsPaused) { return; }
        if (pState.cutscenes) return;
        RestoreTimeScale();
        FlashWhileInvincible();
        UpdateCameraYDampForPlayerFall();
        if (pState.dashing || !pState.alive) { return; }
        if (InputEnable && pState.alive)
        {
            if (unlockedHeal)
            {
                Heal();
            }
            if (!canMove || pState.healing) { return; }
            GetInput();
            ToggleMap();
            if (!isWallJumping)
            {
                Move();
                Flip();
                Jump();
            }
            if (openMap) { return; }
            if (unlockedWallJump)
            {
                WallSlide();
                WallJump();
            }
            UpdateJumpVariables();
            if (unlockedDash)
            {
                StartDash();
            }
            if (unlockedCastSpell)
            {
                CastSpell();
            }
            isFalling();
            Attack();
            Recoil();
        }
    }
    void UpdateCameraYDampForPlayerFall()
    {
        if (rb.velocity.y < playerFallSpeedThreshold && !CameraManager.instance.isLerpingYDamping && !CameraManager.instance.hasLerpedYDamping)
        {
            StartCoroutine(CameraManager.instance.LerpYDamping(true));
        }
        if (rb.velocity.y >= 0 && !CameraManager.instance.isLerpingYDamping && CameraManager.instance.hasLerpedYDamping)
        {
            CameraManager.instance.hasLerpedYDamping = false;
            StartCoroutine(CameraManager.instance.LerpYDamping(false));
        }
    }
    #endregion
    #region Input
    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal"); 
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        openMap = Input.GetKey(KeyCode.M);
        if (Input.GetButtonDown("CastSpell")|| Input.GetButtonDown("Healing"))
        {
            castOrHealTimer += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(10);
        }
    }
    #endregion
    #region Horizontal Movement
    public void Flip()
    {
        if (xAxis < 0 && isFacingRight)
        {
            //transform.eulerAngles = new Vector2(0, 180);
            Turn();
            if (Grounded())
            {
                anim.SetTrigger("Rotating");
            }
        }
        else if (xAxis > 0 && !isFacingRight)
        {
            //transform.eulerAngles = new Vector2(0, 0);
            Turn();
            if (Grounded())
            {
                anim.SetTrigger("Rotating");
            }
        }
    }
    void Turn()
    {
        if (isFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
            pState.lookingRight = false;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
            pState.lookingRight = true;
        }
    }
    void Move()
    {
        if (pState.healing || !canMove)
        {
            rb.velocity = new Vector2(0, 0);
        }
        if (canMove)
        {
            rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
            anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
            PlayerParticles.WaterFootPrint();
        }
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            anim.SetTrigger("StopTrigger");
            anim.ResetTrigger("Rotating");
            anim.SetBool("Walking", false);
        }
        else
        {
            anim.ResetTrigger("StopTrigger");
        }
    }
    #endregion
    #region Dash
    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (Grounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash() //the dash action the player performs
    {
        canDash = false;
        pState.dashing = true;
        attackable = false;
        InputEnable = false;
        anim.SetTrigger("Dashing");
        audioManager.PlayVfx(audioManager.vfx[1]);
        Instantiate(StartdashEffect, transform.position, Quaternion.identity);
        PlayerParticles.WaterSplash();
        gameObject.layer = LayerMask.NameToLayer("Decoration");
        yield return new WaitForSeconds(0.1f);
        float dashDirection = (transform.rotation.eulerAngles.y == 180f) ? -1f : 1f;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0);
        Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(0.2f);
        gameObject.layer = LayerMask.NameToLayer("Player");
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        InputEnable = true;
        attackable = true;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion
    #region Attack
    void Attack()
    {
        float verticalDirection = Input.GetAxisRaw("Vertical");
        timeSinceAttack += Time.deltaTime;
        ResetCombo();
        if (attack && timeSinceAttack >= AtkInterval && attackable && !pState.casting && !Walled())
        {
            timeSinceAttack = 0;
            audioManager.PlayVfx(audioManager.vfx[0]);
            if (verticalDirection > 0)
            {
                anim.SetTrigger("AttackUp");
                PlayerParticles.UpSlash();
            }
            else if (verticalDirection < 0 && !Grounded())
            {
                anim.SetTrigger("AttackDown");
                PlayerParticles.DownSlash();
            }
            else
            {
                if (!Grounded())
                {
                    anim.SetTrigger("Attack");
                    PlayerParticles.ForwardSlash();
                    return;
                } 
                PerformCombo();
            }
        }
    }
    private void ResetCombo()
    {
        if (timeSinceAttack > comboResetTime)
        {
            comboStep = 0;
        }
    }

    private void PerformCombo()
    {
        if (comboStep == 0)
        {
            anim.SetTrigger("Attack");
            PlayerParticles.ForwardSlash();
            comboStep++;
        }
        else if (comboStep == 1  && timeSinceAttack < comboResetTime)
        {
            anim.SetTrigger("SecondAtk");
            PlayerParticles.Combo2Slash();
            comboStep++;
        }
        else if (comboStep == 2  && timeSinceAttack < comboResetTime)
        {
            anim.SetTrigger("ThirdAtk");
            PlayerParticles.Combo3Slash();
            comboStep = 0;
        }
    }
    private void attackForward(int comboStep)
    {
        float finalDamageMultiplier = 1.0f; // Hệ số sát thương mặc định cho đòn đầu tiên
        float recoilSpeedMultiplier = 1.0f; // Hệ số hồi phản lực mặc định cho đòn đầu tiên

        // Thiết lập hệ số sát thương và hệ số hồi phản lực cho từng bước combo
        if (comboStep == 1) // Đòn thứ nhất
        {
            finalDamageMultiplier = 1.0f;  // 100% sát thương
            recoilSpeedMultiplier = 1.0f;  // Hệ số phản lực thông thường (không thay đổi)
        }
        else if (comboStep == 2) // Đòn thứ hai
        {
            finalDamageMultiplier = 1.25f;  // 125% sát thương
            recoilSpeedMultiplier = 2.0f;   // Tăng tốc độ hồi phản lực (gấp đôi so với đòn đầu)
        }
        else if (comboStep == 3) // Đòn thứ ba
        {
            finalDamageMultiplier = 1.5f;  // 150% sát thương
            recoilSpeedMultiplier = 4.0f;  // Tăng hồi phản lực rất mạnh (gấp 4 lần so với đòn đầu)
        }

        // Tăng sát thương dựa trên hệ số
        float finalDamage = damage * finalDamageMultiplier; // Tính toán sát thương cuối cùng dựa trên hệ số combo
        float recoilFinalSpeed = recoilXSpeed * recoilSpeedMultiplier; // Tính toán tốc độ hồi phản lực cuối cùng
        int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
        Hit(attackForwardPoint, SideAttackRange, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilFinalSpeed, finalDamage);
        //Debug.Log("Sat thuong gay ra la " + finalDamage);
        StartCoroutine(attackCoroutine(attackeffectdelay, AtkInterval));
    }
    private void attackUp()
    {
        Hit(UpAttackPoint, UpAttackRange, ref pState.recoilingY ,Vector2.up, recoilYSpeed,damage);
        StartCoroutine(attackCoroutine(attackeffectdelay, AtkInterval));
    }
    private void attackDown()
    {
        Hit(DownAttackPoint, DownAttackRange, ref pState.recoilingY, Vector2.down, recoilYSpeed, damage);
        StartCoroutine(attackCoroutine(attackeffectdelay, AtkInterval));
    }

    IEnumerator attackCoroutine(float effectdelay, float atkInterval)
    {
        yield return new WaitForSeconds(effectdelay);
        attackable = false;
        yield return new WaitForSeconds(atkInterval);
        attackable = true;
    }

    void Hit(Transform _attackTransform, float _attackrange, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength, float damage)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapCircleAll(_attackTransform.position, _attackrange, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();
        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            Projectile obj = objectsToHit[i].GetComponent<Projectile>();
            if (e && !hitEnemies.Contains(e))
            {
                e.EnemyGetsHit(damage, _recoilDir, _recoilStrength);
                hitEnemies.Add(e);
                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
            else if(obj)
            {
                if (objectsToHit[i].CompareTag("Projectile"))
                {
                   Destroy(obj.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(attackForwardPoint.position, SideAttackRange);
        Gizmos.DrawWireSphere(UpAttackPoint.position, UpAttackRange);
        Gizmos.DrawWireSphere(DownAttackPoint.position, DownAttackRange);
    }

    #endregion
    #region Recoil
    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            AirJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }
    #endregion
    #region TakeDamage Respawn
    public void TakeDamage(float _damage)
    {
        if (pState.alive)
        {
            audioManager.PlayVfx(audioManager.vfx[2]);
            Health -= Mathf.RoundToInt(_damage);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }
        }
    }
    IEnumerator StopTakingDamage()
    {
        pState.Invincible = true;
        canMove = false;
        anim.SetTrigger("isHurt");
        Mana += manaGain;
        GameObject _bloodParticles = Instantiate(Blood, transform.position, Quaternion.identity);
        Destroy(_bloodParticles, 1.5f);
        yield return new WaitForSeconds(1f);
        canMove = true;
        yield return new WaitForSeconds(0.25f);
        pState.Invincible = false;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        audioManager.PlayVfx(audioManager.vfx[5]);
        GameObject _bloodSpurtParticles = Instantiate(Blood, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("isDead");
        GlobalController.instance.DecreasePlayerScoreByHalf();
        yield return new WaitForSeconds(1f);
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());
        yield return new WaitForSeconds(1f);
        Instantiate(GlobalController.instance.shade, transform.position, Quaternion.identity);
        SaveData.Instance.SavePlayerData();
    }
    IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.1f);
        canFlash = true;
    }

    void FlashWhileInvincible()
    {
        if (pState.Invincible && !pState.cutscenes)
        {
            if (Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Instantiate(HurtEffect, transform.position, Quaternion.identity);
        if (_delay > 0)
        {
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = _newTimeScale;
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        restoreTime = true;
    }
    public void Respawned()
    {
        if (!pState.alive)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            GetComponent<BoxCollider2D>().enabled = true;
            pState.alive = true;
            halfMana = true;
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
            Mana = 0;
            Health = maxHealth;
            anim.Play("AshenOne_Idle");
        }
    }
    public void RestoreMana()
    {
        halfMana = false;
        UIManager.Instance.SwitchMana(UIManager.ManaState.FullMana);
    }
    #endregion
    #region CharacterSetting
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }
    public float Mana
    {
        get { return mana; }
        set
        {
            //if mana stats change
            if (mana != value)
            {
                if (!halfMana)
                {
                    mana = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    mana = Mathf.Clamp(value, 0, 0.5f);
                }
                manaStorage.fillAmount = Mana;
            }
        }
    }
    public bool Grounded()
    {
        if (Physics2D.Raycast(FootCheckPoint.position, Vector2.down, FootCheckY, whatIsGround)
            || Physics2D.Raycast(FootCheckPoint.position + new Vector3(FootCheckX, 0, 0), Vector2.down, FootCheckY, whatIsGround)
            || Physics2D.Raycast(FootCheckPoint.position + new Vector3(-FootCheckX, 0, 0), Vector2.down, FootCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsOnWater()
    {
        if (Physics2D.Raycast(FootCheckPoint.position, Vector2.down, FootCheckY, WhatIsWater)
            || Physics2D.Raycast(FootCheckPoint.position + new Vector3(FootCheckY, 0, 0), Vector2.down, FootCheckY, WhatIsWater)
            || Physics2D.Raycast(FootCheckPoint.position + new Vector3(-FootCheckY, 0, 0), Vector2.down, FootCheckY, WhatIsWater))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    public void ResetToDefault()
    {
        halfMana = false;
        Health = maxHealth;
        Mana = 0.5f;
        heartShards = 0;

        unlockedWallJump = false;
        unlockedDash = false;
        unlockedVarJump = false;
        unlockedHeal = false;
        unlockedCastSpell = false;
    }
    #endregion
    #region Heal
    void Heal()
    {
        if (Input.GetButton("Healing") && castOrHealTimer <= 0.05f && Health < maxHealth && Grounded() && !pState.dashing && !pState.Invincible && canMove && Mana >= manaDrainSpeed && !Walled())
        {
            pState.healing = true;
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("isHealing", true);
            HealingEffect.SetActive(true);
            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal && anim.GetBool("isHealing"))
            {
                Health++;
                healTimer = 0;
                anim.SetBool("isHealing", false);
                anim.SetTrigger("isHealed");
                StartCoroutine(HealedDelay());
                pState.healing = false;
            }
            //drain mana
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            canMove = true;
            pState.healing = false;
            healTimer = 0;
            anim.SetBool("isHealing", false);
            anim.ResetTrigger("isHealed");
            HealingEffect.SetActive(false);
        }
        if(!Input.GetButtonDown("Healing"))
        {
            castOrHealTimer = 0;
        }
    }
    IEnumerator HealedDelay()
    {
        Instantiate(HealedEffect, transform);
        yield return new WaitForSeconds(1f);
    }
    #endregion
    #region CastSpell
    void CastSpell()
    {
        if (Input.GetButtonDown("CastSpell") && castOrHealTimer <= 0.05f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost && !Walled())
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
        if (!Input.GetButtonDown("CastSpell"))
        {
            castOrHealTimer = 0;
        }
        if (Grounded())
        {
            //disable downspell if on the ground
            downSpell.SetActive(false);
            downSpellEffect.SetActive(false);
        }
        //if down spell is active, force player down until grounded
        if (downSpell.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }

    IEnumerator CastCoroutine()
    {
        Mana -= manaSpellCost;
        //side cast
        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            audioManager.PlayVfx(audioManager.vfx[6]);
            anim.SetBool("CastingSide", true);
            yield return new WaitForSeconds(0.15f);
            GameObject _SideSpell = Instantiate(sideSpell, attackForwardPoint.position, Quaternion.identity);

            //flip fireball
            if (pState.lookingRight)
            {
                _SideSpell.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _SideSpell.transform.eulerAngles = new Vector2(_SideSpell.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;
            InputEnable = false;
            yield return new WaitForSeconds(0.35f);
            InputEnable = true;
            anim.SetBool("CastingSide", false);
        }

        //up cast
        else if (yAxis > 0)
        {
            audioManager.PlayVfx(audioManager.vfx[7]);
            anim.SetBool("isCastingUp", true);
            yield return new WaitForSeconds(0.2f);
            Instantiate(upSpell, transform);
            rb.velocity = Vector2.zero;
            InputEnable = false;
            yield return new WaitForSeconds(0.35f);
            InputEnable = true;
            anim.SetBool("isCastingUp", false);
        }

        //down cast
        else if (yAxis < 0 && !Grounded())
        {
            audioManager.PlayVfx(audioManager.vfx[7]);
            anim.SetBool("CastingDown", true);
            yield return new WaitForSeconds(0.15f);
            InputEnable = false;
            downSpell.SetActive(true);
            downSpellEffect.SetActive(true);
            yield return new WaitForSeconds(0.35f);
            InputEnable = true;
            anim.SetBool("CastingDown", false);
        }

        pState.casting = false;
    }

    private void OnTriggerEnter2D(Collider2D _other) //for up and down cast spell
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyGetsHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }
    #endregion
    #region Vertical Movement
    void isFalling()
    {

        if (rb.velocity.y < fallThreshold && !Grounded() && !Walled())
        {
            pState.Falling = true;
            attackable = false;
            anim.SetBool("Landing", true);
            anim.SetBool("Landed", false);
            if (currentFallSpeed > _maxFallSpeed)
            {
                currentFallSpeed += fallSpeedIncrease * Time.deltaTime;
            }
            else
            {
                currentFallSpeed = _maxFallSpeed;
            }
            rb.velocity = new Vector2(rb.velocity.x, currentFallSpeed);
        }
        else if (Grounded())
        {
            pState.Falling = false;
            attackable = true;
            if (anim.GetBool("Landing"))
            {
                anim.SetBool("Landing", false);
                anim.SetBool("Landed", true);
                if (!landingSoundPlayed)
                {
                    landingSoundPlayed = true;
                    audioManager.PlayVfx(audioManager.vfx[3]);
                }
                StartCoroutine(LandingDelayCoroutine());
            }
            else
            {
                anim.SetBool("Landing", false);
                anim.SetBool("Landed", false);
                landingSoundPlayed = false;
            }
        }
    }
    IEnumerator LandingDelayCoroutine()
    {
        canMove = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        Instantiate(landEffect, transform);
        yield return new WaitForSecondsRealtime(0.15f);
        currentFallSpeed = 0.0f;
        canMove = true;
        anim.SetBool("Landed", false);
        anim.ResetTrigger("DoubleJump");
        JumpEffectCreated = 0;
        PlayerParticles.JumpSplashCounted = 0;
        PlayerParticles.DoubleJumpVfxCounted = 0;
    }
    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            pState.Jumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        if (!pState.Jumping)
        {
            if (JumpBufferCounter > 0 && CoyoteTimeCounter > 0)
            {
                pState.Jumping = true;

                audioManager.PlayVfx(audioManager.vfx[4]);

                if (JumpEffectCreated == 0)
                { 
                    Instantiate(JumpEffect, transform.position, Quaternion.identity);
                    JumpEffectCreated++;
                }

                PlayerParticles.WaterJumpVFX();

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            }
            else if (!Grounded() && AirJumpCounter < maxAirJumps && Input.GetButtonDown("Jump") && unlockedVarJump)
            {
                pState.Jumping = true;

                audioManager.PlayVfx(audioManager.vfx[4]);

                AirJumpCounter++;

                anim.SetTrigger("DoubleJump");

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                PlayerParticles.WingVfx();

                Instantiate(doubleJumpEffect, transform);

                PlayerParticles.WaterJumpVFX();
            }
        }
        anim.SetBool("Jumping", !Grounded());
    }
    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            CoyoteTimeCounter = coyoteTime;

            pState.Jumping = false;

            AirJumpCounter = 0;
        }
        else
        {
            CoyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            JumpBufferCounter = JumpBufferFrames;
        }
        else
        {
            JumpBufferCounter--;
        }
    }
    void WallSlide()
    {
        if (Walled() && !Grounded() && xAxis != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            anim.SetBool("isWallJump", false);
            anim.SetBool("isWallSlide", true);
        }
        else
        {
            isWallSliding = false;
            anim.SetBool("isWallSlide", false);
        }
    }
    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = !pState.lookingRight ? 1 : -1;
            CancelInvoke(nameof(StopWallJumping));
        }

        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            audioManager.PlayVfx(audioManager.vfx[4]);
            isWallJumping = true;

            anim.SetBool("isWallJump", true);
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            dashed = false;
            AirJumpCounter = 0;

            pState.lookingRight = !pState.lookingRight;
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }
    void StopWallJumping()
    {
        isWallJumping = false;
        transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
    }
    #endregion
    #region Interact
    public void Interact()
    {
        if(Grounded() && Input.GetKeyDown(KeyCode.E) && !pState.casting && !pState.Falling && !pState.Jumping && !pState.Invincible  && !pState.dashing && !pState.healing)
        {
            rb.velocity = Vector3.zero;
            anim.SetTrigger("isInteract");
            InputEnable = false;
            StartCoroutine(AnimationDelay());
        }
    }
    public IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(1.2f);
        InputEnable = true;
        anim.ResetTrigger("isInteract");
    }

    public void WalkIntoDoor()
    {
        StartCoroutine(WalkIntoDoorCoroutine());
    }

    IEnumerator WalkIntoDoorCoroutine()
    {
        pState.cutscenes = true;
        gameObject.layer = LayerMask.NameToLayer("Decoration");
        yield return new WaitForSeconds(1.5f);
        InputEnable = false;
        rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
        yield return new WaitForSeconds(1.2f);
        gameObject.layer = LayerMask.NameToLayer("Player");
        pState.cutscenes = false;
        rb.velocity = new Vector3(0, 0, 0);
        Move();
        InputEnable = true;
    }

    public void TheEnd()
    {
        StartCoroutine(Ending());
    }

    IEnumerator Ending()
    {
        if (Grounded() && Input.GetKeyDown(KeyCode.E) && !pState.casting && !pState.Falling && !pState.Jumping && !pState.Invincible && !pState.dashing && !pState.healing)
        {
            pState.cutscenes = true;
            rb.velocity = Vector3.zero;
            //gravity = 0;
            InputEnable = false;
            anim.SetTrigger("Ending");
            yield return new WaitForSeconds(0.25f);
            Vector3 targetPosition = new Vector3(transform.position.x,TheEndMapTranstition.Instance.transform.position.y, transform.position.z);
            float riseSpeed = 2f; // Adjust this value to change the speed of rising
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, riseSpeed * Time.deltaTime);
                yield return null; // Wait for the next frame
            }
            EndingEffect.SetActive(true);
            yield return new WaitForSeconds(4f);
        }
        anim.ResetTrigger("Ending");
    }
    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        pState.Invincible = true;

        //If exit direction is upwards
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.Invincible = false;
    }
    #endregion
    #region Map
    void ToggleMap()
    {
        if (openMap && !pState.dashing && !pState.healing && !pState.casting && !pState.cutscenes && !pState.Jumping && !pState.Falling && !pState.Invincible)
        {
            anim.SetBool("isOpenMap", true);
            canMove = false;
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            StartCoroutine(CloseMap());
        }
    }

    IEnumerator CloseMap()
    {
        anim.SetBool("isOpenMap", false);
        UIManager.Instance.mapHandler.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        canMove = true;
    }
    #endregion
}
