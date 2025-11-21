using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.Mathematics;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("スプライト設定")]
    public Sprite standingRightSprite;
    public Sprite standingLeftSprite;
    public Sprite[] runRightSprites;
    public Sprite[] runLeftSprites;
    public float runAnimSpeed = 0.1f;
    private int runAnimIndex = 0;
    private float runAnimTimer = 0;
    
    [Header("投げアニメーション")]
    public Sprite[] throwAnimRightSprites; // 右向き投げるアニメーション
    public Sprite[] throwAnimLeftSprites;  // 左向き投げるアニメーション
    public float throwAnimSpeed = 0.1f;

    [Header("チャージアニメーション")]
    public Sprite chargePoseRightSprite;      // 構え画像（右向き）
    public Sprite chargePoseLeftSprite;       // 構え画像（左向き）
    public Sprite[] chargeAnimRightSprites;   // 2秒以上の交互アニメーション（右向き）
    public Sprite[] chargeAnimLeftSprites;
    public Sprite[] chargeAnimRightSprites2;  // 2.6秒以上のアニメーション（右向き）
    public Sprite[] chargeAnimLeftSprites2;   // 2.6秒以上のアニメーション（左向き）
    public float chargeAnimSpeed = 0.2f;      // 交互表示の速度

    private float chargeAnimTimer = 0;
    private int chargeAnimIndex = 0;

    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    private bool isSpeedBoosted = false;
    private float originalMoveSpeed;

    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float hitIntervalSec = 0.3f;

    [Header("ショット設定")]
    [SerializeField] private bool isBlackBoosted = false;
    private float chargeTimeMultiplier = 1f;

    public GameObject canprefab;
    public GameObject boxPrefab;
    public Transform throwPoint;
    public float throwForce = 10f;
    private float spacePressTime;
    private float holdTime;
    private bool isCharging = false;
    private bool isSpeedReduced = false;
    public AudioClip throwSound;
    [SerializeField, Range(0f, 1f)]
    private float throwSoundVolume = 1f;

    [Header("体力設定")]
    public int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private Slider HPbar;
    [SerializeField] private float damageflashDuration = 0.1f;
    [SerializeField] private int damageflashCount = 4;
    public AudioClip damageSound;
    [SerializeField, Range(0f, 1f)]
    private float damageSoundVolume = 1f;
    public AudioClip healSound;
    [SerializeField, Range(0f, 1f)]
    private float healSoundVolume = 1f;
    public AudioClip powerupSound;
    [SerializeField, Range(0f, 1f)]
    private float powerupSoundVolume = 1f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isDead = false;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private bool facingRight = true;
    
    private int throwAnimIndex = 0;         // アニメーションのインデックス
    private float throwAnimTimer = 0f;      // アニメーションのタイマー
    private bool isThrowing = false;

    // effect_sound
    public AudioClip[] enegyDrinkSound;
    private enum DrinkSoundType { first, second, third, forth, first_shot, second_shot, third_shot }
    private DrinkSoundType currentChargeType = DrinkSoundType.first;
    private bool isSoundPlaying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        UpdateHPBar();
        originalMoveSpeed = moveSpeed;

        // AudioSourceの設定
        audioSource = gameObject.AddComponent<AudioSource>();
        sr.sprite = standingRightSprite;
    }

    void Update()
    {
        // --- 投げアニメーション処理 ---
        if (isThrowing)
        {
            throwAnimTimer += Time.deltaTime;
            if (throwAnimTimer >= throwAnimSpeed)
            {
                throwAnimTimer = 0;
                throwAnimIndex++;

                if (facingRight && throwAnimRightSprites.Length > 0)
                {
                    sr.sprite = throwAnimRightSprites[throwAnimIndex % throwAnimRightSprites.Length];
                }
                else if (!facingRight && throwAnimLeftSprites.Length > 0)
                {
                    sr.sprite = throwAnimLeftSprites[throwAnimIndex % throwAnimLeftSprites.Length];
                }
            }

            // アニメーション終了判定
            if ((facingRight && throwAnimIndex >= throwAnimRightSprites.Length) || 
                (!facingRight && throwAnimIndex >= throwAnimLeftSprites.Length))
            {
                isThrowing = false;
                throwAnimIndex = 0;
                sr.sprite = facingRight ? standingRightSprite : standingLeftSprite; 
            }
            // 投げ動作中はここでリターンせず、移動などの入力を受け付ける場合は下に続ける
            // ここでは投げモーション優先としておきますが、必要ならreturnを削除してください
            // return; 
        }

        if (isDead) return;

        // --- 移動入力 ---
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0) facingRight = true;
        else if (moveInput < 0) facingRight = false;

        // --- ジャンプ判定 ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }

        // =========================================================
        // ショット・チャージ処理
        // =========================================================

        // 1. 押し始め
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressTime = Time.time;
            isCharging = true;

            currentChargeType = DrinkSoundType.first;
            isSoundPlaying = false;
            audioSource.Stop();
        }

        // 2. 離した時 (発射)
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isSpeedReduced)
            {
                moveSpeed = originalMoveSpeed;
                isSpeedReduced = false;
            }

            isCharging = false;
            isThrowing = true; // 投げアニメーション開始フラグ
            moveSpeed = originalMoveSpeed;
            holdTime = (Time.time - spacePressTime) * chargeTimeMultiplier;

            // 音停止
            audioSource.Stop();
            audioSource.loop = false;
            isSoundPlaying = false;

            // 発射処理
            DrinkSoundType shotSoundToPlay = DrinkSoundType.first_shot;
            bool shouldPlayShotSound = false;

            if (isBlackBoosted)
            {
                shouldPlayShotSound = true;
                if (holdTime >= 2.6f) {
                    ThrowBox(true, true, new Vector2(1f, 0.25f)); shotSoundToPlay = DrinkSoundType.third_shot;
                }
                else if (holdTime >= 1.6f) {
                    ThrowBox(true, false, new Vector2(1f, 0.8f)); ThrowBox(true, false, new Vector2(1f, 0.1f));
                    ThrowBox(true, false, new Vector2(1f, -0.05f)); shotSoundToPlay = DrinkSoundType.second_shot;
                }
                else {
                    ThrowBox(true, false, new Vector2(1f, 0.25f)); shotSoundToPlay = DrinkSoundType.first_shot;
                }
            }
            else
            {
                if (holdTime >= 2.6f) {
                    ThrowBox(true, true, new Vector2(1f, 0.25f));
                    shotSoundToPlay = DrinkSoundType.third_shot;
                    shouldPlayShotSound = true;
                }
                else if (holdTime >= 1.6f) {
                    ThrowBox(true, false, new Vector2(1f, 0.8f)); ThrowBox(true, false, new Vector2(1f, 0.1f));
                    ThrowBox(true, false, new Vector2(1f, -0.05f));
                    shotSoundToPlay = DrinkSoundType.second_shot;
                    shouldPlayShotSound = true;
                }
                else if (holdTime >= 0.6f) {
                    ThrowBox(true, false, new Vector2(1f, 0.25f));
                    shotSoundToPlay = DrinkSoundType.first_shot;
                    shouldPlayShotSound = true;
                }
                else {
                    ThrowBox(false, false, new Vector2(1f, 0.25f));
                    shotSoundToPlay = DrinkSoundType.first_shot;
                    shouldPlayShotSound = false;
                }
            }

            if (shouldPlayShotSound)
            {
                audioSource.PlayOneShot(enegyDrinkSound[(int)shotSoundToPlay]);
            }
        }

        // 3. チャージ中の処理 (音とアニメーション)
        if (isCharging)
        {
            holdTime = (Time.time - spacePressTime) * chargeTimeMultiplier;
            DrinkSoundType targetChargeType;

            // --- 音のランク管理 ---
            if (holdTime >= 2.6f)
            {
                targetChargeType = DrinkSoundType.forth;
                if (!isSpeedReduced) { moveSpeed = originalMoveSpeed * 0.7f; isSpeedReduced = true; }
            }
            else if (holdTime >= 1.6f)
            {
                targetChargeType = DrinkSoundType.third;
                if (!isSpeedReduced) { moveSpeed = originalMoveSpeed * 0.5f; isSpeedReduced = true; }
            }
            else if (holdTime >= 0.6f)
            {
                targetChargeType = DrinkSoundType.second;
            }
            else
            {
                targetChargeType = DrinkSoundType.first;
            }

            // 音の切り替え
            if (currentChargeType != targetChargeType)
            {
                audioSource.Stop();
                currentChargeType = targetChargeType;

                if (targetChargeType != DrinkSoundType.first)
                {
                    audioSource.clip = enegyDrinkSound[(int)targetChargeType];
                    audioSource.loop = true;
                    audioSource.Play();

                    int shotIndex = (int)targetChargeType + 3;
                    if (shotIndex < enegyDrinkSound.Length)
                    {
                        audioSource.PlayOneShot(enegyDrinkSound[shotIndex]);
                    }
                }
                isSoundPlaying = true;
            }

            // --- チャージ中のアニメーション ---
            if (holdTime >= 2.6f)
            {
                // 2.6秒以上
                chargeAnimTimer += Time.deltaTime;
                if (chargeAnimTimer >= chargeAnimSpeed)
                {
                    chargeAnimTimer = 0;
                    chargeAnimIndex++;
                }
                
                if (facingRight && chargeAnimRightSprites2.Length > 0)
                    sr.sprite = chargeAnimRightSprites2[chargeAnimIndex % chargeAnimRightSprites2.Length];
                else if (!facingRight && chargeAnimLeftSprites2.Length > 0)
                    sr.sprite = chargeAnimLeftSprites2[chargeAnimIndex % chargeAnimLeftSprites2.Length];
            }
            else if (holdTime >= 1.6f)
            {
                // 1.6秒以上
                chargeAnimTimer += Time.deltaTime;
                if (chargeAnimTimer >= chargeAnimSpeed)
                {
                    chargeAnimTimer = 0;
                    chargeAnimIndex++;
                }

                if (facingRight && chargeAnimRightSprites.Length > 0)
                    sr.sprite = chargeAnimRightSprites[chargeAnimIndex % chargeAnimRightSprites.Length];
                else if (!facingRight && chargeAnimLeftSprites.Length > 0)
                    sr.sprite = chargeAnimLeftSprites[chargeAnimIndex % chargeAnimLeftSprites.Length];
            }
            else
            {
                // 1.6秒未満 (構え)
                sr.sprite = facingRight ? chargePoseRightSprite : chargePoseLeftSprite;
                chargeAnimIndex = 0;
                chargeAnimTimer = 0;
            }
        }
        else
        {
            // --- チャージしていない時の通常アニメーション ---
            // (投げ動作中は上部で処理しているので、ここでは投げ中でない場合のみ処理)
            if (!isThrowing)
            {
                if (Mathf.Abs(moveInput) > 0.1f)
                {
                    // 走り
                    runAnimTimer += Time.deltaTime;
                    if (runAnimTimer >= runAnimSpeed)
                    {
                        runAnimTimer = 0;
                        runAnimIndex++;
                    }

                    if (facingRight && runRightSprites.Length > 0)
                        sr.sprite = runRightSprites[runAnimIndex % runRightSprites.Length];
                    else if (!facingRight && runLeftSprites.Length > 0)
                        sr.sprite = runLeftSprites[runAnimIndex % runLeftSprites.Length];
                }
                else
                {
                    // 停止
                    sr.sprite = facingRight ? standingRightSprite : standingLeftSprite;
                    runAnimIndex = 0;
                    runAnimTimer = 0;
                }
            }
        }
    }

    // --- メソッド定義 ---

    void ThrowBox(bool explosive, bool isBox, Vector2 throwDir)
    {
        GameObject prefabToThrow = isBox ? boxPrefab : canprefab;
        if (prefabToThrow == null || throwPoint == null) return;

        GameObject obj = Instantiate(prefabToThrow, throwPoint.position, Quaternion.identity);
        audioSource.PlayOneShot(throwSound, throwSoundVolume);

        Rigidbody2D objRb = obj.GetComponent<Rigidbody2D>();
        if (objRb != null)
        {
            float direction = facingRight ? 1f : -1f;
            float adjustedThrowForce = throwForce * (moveSpeed / originalMoveSpeed);

            Vector2 forceDir = new Vector2(direction * throwDir.x, throwDir.y);
            if (Mathf.Abs(throwDir.y) < 0.05f) forceDir.y = 0.5f;

            objRb.AddForce(forceDir.normalized * adjustedThrowForce, ForceMode2D.Impulse);
            objRb.linearVelocity += new Vector2(rb.linearVelocity.x * 0.5f, 0);
        }
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead || isInvincible) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHPBar();
        audioSource.PlayOneShot(damageSound, damageSoundVolume);
        rb.AddForce(transform.right * -400.0f);
        Debug.Log("ダメージを受けた");
        DamageFlash();

        isInvincible = true;
        StartCoroutine(InvincibleCoroutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void DamageFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < damageflashCount; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(damageflashDuration);

            sr.color = Color.white;
            yield return new WaitForSeconds(damageflashDuration);
        }
    }

    public void SpeedBoost(float boostAmount, float duration)
    {
        if (!isSpeedBoosted)
        {
            StartCoroutine(SpeedBoostCoroutine(boostAmount, duration));
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHPBar();
        audioSource.PlayOneShot(healSound, healSoundVolume);
        Debug.Log($"回復！ 現在のHP: {currentHealth}");
    }

    private IEnumerator SpeedBoostCoroutine(float boostAmount, float duration)
    {
        isSpeedBoosted = true;
        moveSpeed *= boostAmount;
        Debug.Log("スピードアップ！");

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        isSpeedBoosted = false;
        Debug.Log("元のスピードに戻った");
    }

    IEnumerator InvincibleCoroutine()
    {
        yield return new WaitForSeconds(hitIntervalSec);
        isInvincible = false;
    }

    public void ActivateBlackBoost(float duration, float chargeMultiplier)
    {
        if (!isBlackBoosted)
        {
            StartCoroutine(BlackBoostCoroutine(duration, chargeMultiplier));
        }
    }

    private IEnumerator BlackBoostCoroutine(float duration, float chargeMultiplier)
    {
        isBlackBoosted = true;
        chargeTimeMultiplier = chargeMultiplier;
        Debug.Log("ショット強化！");
        audioSource.PlayOneShot(powerupSound, powerupSoundVolume);
        yield return new WaitForSeconds(duration);

        chargeTimeMultiplier = 1f;
        isBlackBoosted = false;
        Debug.Log("ショット強化終了");
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Debug.Log("Player Died");

        // 1.5秒後にゲームオーバー遷移処理を呼ぶ
        Invoke("TransitionToGameOver", 1.5f);
    }

    void TransitionToGameOver()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
        else
        {
            SceneManager.LoadScene("Bad");
        }
    }

    void UpdateHPBar()
    {
        if (HPbar != null)
        {
            HPbar.value = (float)currentHealth / maxHealth;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}