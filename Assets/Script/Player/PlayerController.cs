using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using System.IO;
using static Unity.VisualScripting.Member;

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
    [Space(5)]

    [Header("Ground Check Settings:")]
    public Transform FootCheckPoint;
    [SerializeField] private float FootCheckY = 0.2f;
    [SerializeField] private float FootCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask WhatIsWater;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    public bool canDash = true, dashed;
    private float gravity;
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
    private bool restoreTime;
    private float restoreTimeSpeed;
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
    private float healTimer;
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
    private float castOrHealTimer;
    [Space(5)]
    [Header("Camera Stuff")]
    [SerializeField] private float playerFallSpeedThreshold = -10;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    private float wallJumpingDirection;
    private float TimeSinceLastJump = 0;

    [Space(5)]

    [Header("Audio")]
    private bool landingSoundPlayed;

    [Header("VFX")]
    private ParticlesController PlayerParticles;
    private ParticlesEffect playerEffect;
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
    private bool openMap;
    public Animator anim;
    private SpriteRenderer sr;
    public float fallThreshold = -12f;
    public bool canMove = true, InputEnable = true;
    public bool isFacingRight = true;
    [SerializeField] private float _maxFallSpeed = -50f;
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
    }
    private void FixedUpdate()
    {
        if (pState.dashing || pState.healing || pState.cutscenes || pState.casting) return;
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
        PlayerParticles = GetComponentInChildren<ParticlesController>();
        playerEffect = GetComponent<ParticlesEffect>();
        gravity = rb.gravityScale;
        Mana = mana;
        manaStorage.fillAmount = Mana;
        Health = maxHealth;
        CheckSave();
        CheckMana();
        if(onHealthChangedCallback != null)
            onHealthChangedCallback.Invoke();
        CheckHealth();
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
            if (!pState.isWallJumping && !pState.isWallSliding)
            {
                Move();
                Flip();
                Jump();
                UpdateJumpVariables();
            }
            if (openMap) { return; }
            if (unlockedWallJump)
            {
                WallSlide();
                WallJump();
            }
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
    private void CheckMana()
    {
        if (halfMana)
        {
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
        }
        else
        {
            UIManager.Instance.SwitchMana(UIManager.ManaState.FullMana);
        }
    }

    private void CheckHealth()
    {
        if (Health <= 0)
        {
            pState.alive = false;
            GlobalController.instance.RespawnPlayer();
        }
    }

    private void CheckSave()
    {
        string playerDataPath = Application.persistentDataPath + "/save.player.data";
        if (File.Exists(playerDataPath))
        {
            SaveData.Instance.LoadPlayerData();
        }
        else
        {
            ResetToDefault();
        }
    }
    #endregion
    #region Input
    private void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal"); // Nhận đầu vào di chuyển ngang (trái: -1, phải: 1, không có đầu vào: 0).
        yAxis = Input.GetAxisRaw("Vertical"); // Nhận đầu vào di chuyển dọc (tương tự ngang).
        attack = Input.GetButtonDown("Attack"); // Kiểm tra nếu nút "Attack" vừa được nhấn.
        openMap = Input.GetKey(KeyCode.M); // Kiểm tra nếu phím "M" (mở bản đồ) đang được giữ.

        // Tăng thời gian đếm nếu nhấn nút cast hoặc heal
        if (Input.GetButtonDown("CastSpell") || Input.GetButtonDown("Healing"))
        {
            castOrHealTimer += Time.deltaTime; // Tăng thời gian đếm theo thời gian mỗi khung hình.
        }
    }
    #endregion
    #region Horizontal Movement
    // Phương thức đảo hướng của nhân vật dựa trên đầu vào
    public void Flip()
    {
        // Nếu di chuyển sang trái trong khi đang đối diện sang phải, lật hướng của nhân vật
        if (xAxis < 0 && isFacingRight)
        {
            Turn(); // Gọi phương thức để lật hướng

            // Nếu nhân vật đang đứng trên mặt đất, kích hoạt hoạt ảnh xoay
            if (Grounded())
            {
                anim.SetTrigger("Rotating");
            }
        }
        // Nếu di chuyển sang phải trong khi đang đối diện sang trái, lật hướng của nhân vật
        else if (xAxis > 0 && !isFacingRight)
        {
            Turn();

            // Kích hoạt hoạt ảnh xoay nếu nhân vật đang đứng trên mặt đất
            if (Grounded())
            {
                anim.SetTrigger("Rotating");
            }
        }
    }

    // Phương thức thực hiện xoay nhân vật theo chiều ngang
    private void Turn()
    {
        // Kiểm tra nếu nhân vật đang đối diện sang phải
        if (isFacingRight)
        {
            // Xoay nhân vật sang trái (180 độ trên trục Y)
            Vector3 rotator = new Vector3(transform.rotation.x, 180, transform.rotation.z);
            Rotation(rotator);
            pState.lookingRight = false; // Cập nhật trạng thái hướng của nhân vật
        }
        else
        {
            // Xoay nhân vật sang phải (0 độ trên trục Y)
            Vector3 rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            Rotation(rotator);
            pState.lookingRight = true;
        }
    }

    // Áp dụng xoay và cập nhật trạng thái hướng của nhân vật
    private void Rotation(Vector3 rotator)
    {
        transform.rotation = Quaternion.Euler(rotator); // Thực hiện xoay nhân vật.
        isFacingRight = !isFacingRight; // Đảo hướng của nhân vật
    }

    // Điều khiển chuyển động của nhân vật dựa trên đầu vào và trạng thái hiện tại
    private void Move()
    {
        // Nếu đang hồi máu hoặc không thể di chuyển, dừng chuyển động theo cả hai chiều
        if (pState.healing || !canMove || pState.casting)
        {
            rb.velocity = new Vector2(0, 0); // Đặt vận tốc về 0.
        }

        // Nếu có thể di chuyển
        if (canMove)
        {
            // Đặt vận tốc ngang của nhân vật dựa trên đầu vào, giữ nguyên vận tốc dọc
            rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);

            // Nếu nhân vật đang di chuyển và đứng trên mặt đất, bật hoạt ảnh đi bộ
            anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());

            // Kích hoạt hiệu ứng hạt (particle) khi đi trên nước hoặc trên đất
            PlayerParticles.FootPrint();
        }

        // Nếu không có đầu vào di chuyển ngang, dừng hoạt ảnh đi bộ và thiết lập lại các trigger
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            anim.SetTrigger("StopTrigger"); // Kích hoạt hoạt ảnh dừng
            anim.ResetTrigger("Rotating"); // Thiết lập lại trigger xoay
            anim.SetBool("Walking", false); // Tắt hoạt ảnh đi bộ
        }
        else
        {
            anim.ResetTrigger("StopTrigger"); // Thiết lập lại trigger dừng nếu đang di chuyển
        }
    }
    #endregion
    #region Dash
    private void StartDash()
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
    private IEnumerator Dash()
    {
        PrepareDash();
        yield return new WaitForSeconds(0.1f);

        ExecuteDash();

        StartCoroutine(EndDash());
    }

    private void PrepareDash()
    {
        canDash = false;
        pState.dashing = true;
        attackable = false;
        InputEnable = false;
        anim.SetTrigger("Dashing");

        PlayDashEffects();
        ChangeLayer("Decoration");
    }

    private void ExecuteDash()
    {
        int dashDirection = GetDirection();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0);
        Debug.Log("Dash started - gravityScale: " + rb.gravityScale);
        Instantiate(playerEffect.dashEffect, transform);
    }

    private IEnumerator EndDash()
    {
        yield return new WaitForSeconds(dashTime);
        ChangeLayer("Player");
        rb.gravityScale = gravity;
        Debug.Log("Dash ended - gravityScale: " + rb.gravityScale);
        pState.dashing = false;
        InputEnable = true;
        attackable = true;
        StartCoroutine(DashCooldown());
    }

    private int GetDirection()
    {
        return pState.lookingRight? 1 : -1;
    }

    private void PlayDashEffects()
    {
        int vfxDirection = GetDirection();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfx[1]);
        Quaternion rotation = vfxDirection == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        PlayerParticles.StartDashEffect(rotation);
    }

    private void ChangeLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion
    #region Attack
    private void Attack()
    {
        float verticalDirection = Input.GetAxisRaw("Vertical");
        timeSinceAttack += Time.deltaTime;
        ResetCombo();
        if (attack && timeSinceAttack >= AtkInterval && attackable && !pState.casting)
        {
            timeSinceAttack = 0;
            AudioManager.instance.PlaySfx(AudioManager.instance.sfx[0]);
            if (verticalDirection > 0)
            {
                anim.SetTrigger("AttackUp");
                playerEffect.SwordSlashUp.Play();
            }
            else if (verticalDirection < 0 && !Grounded())
            {
                anim.SetTrigger("AttackDown");
                playerEffect.SwordSlashDown.Play();
            }
            else
            {
                if (!Grounded())
                {
                    anim.SetTrigger("Attack");
                    playerEffect.SwordSlashForward.Play();
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
            playerEffect.SwordSlashForward.Play();
            comboStep++;
        }
        else if (comboStep == 1  && timeSinceAttack < comboResetTime)
        {
            anim.SetTrigger("SecondAtk");
            playerEffect.SwordSlashCombo2.Play();
            comboStep++;
        }
        else if (comboStep == 2  && timeSinceAttack < comboResetTime)
        {
            anim.SetTrigger("ThirdAtk");
            playerEffect.SwordSlashCombo3.Play();
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
        int _recoilLeftOrRight = GetDirection();
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

    private IEnumerator attackCoroutine(float effectdelay, float atkInterval)
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
                    int direction = GetDirection();
                    int randomZrotation = Random.Range(5, 25);
                    Quaternion rotation = direction == 1 ? Quaternion.Euler(0, 0, randomZrotation) : Quaternion.Euler(0, 180, randomZrotation);
                    Instantiate(playerEffect.enemyVfx, _attackTransform.position, rotation);
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
    private void Recoil()
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
    private void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    private void StopRecoilY()
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
            AudioManager.instance.PlaySfx(AudioManager.instance.sfx[2]);
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
    private IEnumerator StopTakingDamage()
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

    private IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        AudioManager.instance.PlaySfx(AudioManager.instance.sfx[5]);
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
    private IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.1f);
        canFlash = true;
    }

    private void FlashWhileInvincible()
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

    private void RestoreTimeScale()
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

    private IEnumerator StartTimeAgain(float _delay)
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
        return Physics2D.OverlapCircle(wallCheckPoint.position, 0.2f, wallLayer);
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

        GlobalController.instance.playerScore = 0;
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
            DisableDownSpell();
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
        if (IsCastingSideSpell())
        {
            yield return CastSideSpell();
        }
        else if (IsCastingUpSpell())
        {
            yield return CastUpSpell();
        }
        else if (IsCastingDownSpell())
        {
            yield return CastDownSpell();
        }
    }
    private bool IsCastingSideSpell() => yAxis == 0 || (yAxis < 0 && Grounded());

    private bool IsCastingUpSpell() => yAxis > 0;

    private bool IsCastingDownSpell() => yAxis < 0 && !Grounded();

    IEnumerator CastSideSpell()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfx[6]);
        anim.SetBool("CastingSide", true);
        int direction = GetDirection();
        Quaternion rotation = direction == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        Instantiate(playerEffect.StartSideCastVfx,wallCheckPoint.position,rotation);
        yield return new WaitForSeconds(0.2f);
        HandleGravity(true);
        Instantiate(sideSpell, attackForwardPoint.position, rotation);

        yield return CompleteCasting("CastingSide");
    }

    IEnumerator CastUpSpell()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfx[7]);
        anim.SetBool("isCastingUp", true);

        yield return new WaitForSeconds(0.2f);
        HandleGravity(true);
        Instantiate(upSpell, UpAttackPoint.transform);

        yield return CompleteCasting("isCastingUp");
    }

    IEnumerator CastDownSpell()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfx[7]);
        anim.SetBool("CastingDown", true);
        yield return new WaitForSeconds(0.15f);

        downSpell.SetActive(true);
        downSpellEffect.SetActive(true);

        yield return CompleteCasting("CastingDown");
    }

    IEnumerator CompleteCasting(string animationBoolName)
    {
        InputEnable = false;
        yield return new WaitForSeconds(0.35f);
        InputEnable = true;
        pState.casting = false;
        anim.SetBool(animationBoolName, false);
        HandleGravity(false);
    }

    private void DisableDownSpell()
    {
        downSpell.SetActive(false);
        downSpellEffect.SetActive(false);
    }

    private void HandleGravity(bool disableGravity)
    {
        if(true)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.gravityScale = gravity;
        }
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
    private void isFalling()
    {
        if (rb.velocity.y < fallThreshold && !Grounded())
        {
            pState.Falling = true;
            anim.SetBool("Landing", true);
            CalculateFallSpeed();
        }
        else if (Grounded())
        {
            pState.Falling = false;
            attackable = true;
            if (anim.GetBool("Landing") || anim.GetBool("isWallJump"))
            {
                HandleFallingAnim();
                anim.SetBool("Landing", false);
                currentFallSpeed = 0.0f;
                if (!landingSoundPlayed)
                {
                    landingSoundPlayed = true;
                    AudioManager.instance.PlaySfx(AudioManager.instance.sfx[3]);
                }
                StartCoroutine(LandingDelayCoroutine());
            }
            else
            {
                landingSoundPlayed = false;
            }
        }
    }
    private void HandleFallingAnim()
    {
        if(anim.GetBool("Landing"))
        {
            anim.SetBool("Landing", false);
        }
        else if(anim.GetBool("isWallJump"))
        {
            anim.SetBool("isWallJump", false);
        }
    }
    private void CalculateFallSpeed()
    {
        if (currentFallSpeed > _maxFallSpeed)
        {
            currentFallSpeed += _maxFallSpeed * Time.deltaTime;
        }
        else
        {
            currentFallSpeed = _maxFallSpeed;
        }
        rb.velocity = new Vector2(rb.velocity.x, currentFallSpeed);
    }
    private IEnumerator LandingDelayCoroutine()
    {
        canMove = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        Instantiate(playerEffect.landEffect, FootCheckPoint.transform.position,Quaternion.identity);
        yield return new WaitForSecondsRealtime(0.1f);
        canMove = true;
        anim.ResetTrigger("DoubleJump");
    }
    private void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            pState.Jumping = false;
            isFalling();
        }
        if (!pState.Jumping)
        {
            if (JumpBufferCounter > 0 && CoyoteTimeCounter > 0)
            {
                pState.Jumping = true;

                AudioManager.instance.PlaySfx(AudioManager.instance.sfx[4]);

                int direction = GetDirection();

                Quaternion rotation = direction == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

                PlayerParticles.JumpVfx(rotation);

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            }
            else if (!Grounded() && AirJumpCounter < maxAirJumps && Input.GetButtonDown("Jump") && unlockedVarJump && !Walled())
            {
                pState.Jumping = true;

                AudioManager.instance.PlaySfx(AudioManager.instance.sfx[4]);

                AirJumpCounter++;

                anim.SetTrigger("DoubleJump");

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                PlayerParticles.WingVfx();

                Instantiate(playerEffect.doubleJumpEffect, transform);

            }
        }
        anim.SetBool("Jumping", !Grounded());
    }
    private void UpdateJumpVariables()
    {
        if (Grounded())
        {
            CoyoteTimeCounter = coyoteTime;

            pState.Jumping = false;

            AirJumpCounter = 0;

            PlayerParticles.DoubleJumpVfxCounted = 0;

            //JumpEffectCreated = 0;
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
    private void WallSlide()
    {
        if (Walled() && !Grounded() && xAxis != 0)
        {
            pState.isWallSliding = true;
            currentFallSpeed = 0;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            anim.SetBool("isWallJump", false);
            anim.SetBool("isWallSlide", true);
        }
        else
        {
            pState.isWallSliding = false;
            anim.SetBool("isWallSlide", false);
        }
    }
    private void WallJump()
    {
        if (TimeSinceLastJump > 0)
        {
            TimeSinceLastJump -= Time.deltaTime;
            pState.isWallJumping = false;
        }
        if (pState.isWallSliding)
        {
            pState.isWallJumping = false;                 // Tắt trạng thái nhảy tường
            wallJumpingDirection = onWallDirection(); // Xác định hướng nhảy tường dựa trên hướng nhìn của nhân vật
        }

        // Khi nhấn phím nhảy trong khi trượt tường
        if (Input.GetButtonDown("Jump") && pState.isWallSliding && TimeSinceLastJump <= 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.instance.sfx[4]); // Phát âm thanh nhảy

            pState.isWallJumping = true;                  // Đặt trạng thái nhảy tường

            anim.SetBool("isWallJump", true);      // Bật hoạt ảnh nhảy tường

            Vector2 force = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))  // Ngược chiều di chuyển so với lực nhảy tường
                force.x -= rb.velocity.x;

            if (rb.velocity.y < 0)  // Nếu đang rơi, điều chỉnh lực nhảy tường để đảm bảo đủ độ cao
                force.y -= rb.velocity.y;

            rb.AddForce(force, ForceMode2D.Impulse);

            int vfxdirection = onWallDirection();

            Quaternion rotation = vfxdirection == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

            Instantiate(playerEffect.wallJumpVfx, wallCheckPoint.transform.position, rotation);

            dashed = false;                        // Đặt lại trạng thái đã nhảy

            TimeSinceLastJump = wallJumpingDuration;
        }
    }
    
    private int onWallDirection()
    {
        return !pState.lookingRight ? 1 : -1;
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
