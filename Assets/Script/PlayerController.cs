using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.Mathematics;
using System;

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
    private float moveInput;

    // effect_sound
    // ※ここにチャージ音(0~3)とショット音(4~6)を登録してください
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
        if (isDead) return;

        // --- 移動処理 ---
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0) facingRight = true;
        else if (moveInput < 0) facingRight = false;

        // --- アニメーション処理 ---
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            // 移動中 → 走りアニメーション
            runAnimTimer += Time.deltaTime;
            if (runAnimTimer >= runAnimSpeed)
            {
                runAnimTimer = 0;
                runAnimIndex++;
                if (facingRight)
                {
                    if (runRightSprites.Length > 0)
                    {
                        runAnimIndex %= runRightSprites.Length;
                        sr.sprite = runRightSprites[runAnimIndex];
                    }
                }
                else
                {
                    if (runLeftSprites.Length > 0)
                    {
                        runAnimIndex %= runLeftSprites.Length;
                        sr.sprite = runLeftSprites[runAnimIndex];
                    }
                }
            }
        }
        else
        {
            // 停止中
            sr.sprite = facingRight ? standingRightSprite : standingLeftSprite;
            runAnimIndex = 0;
            runAnimTimer = 0;
        }

        // --- ジャンプ・接地判定 ---
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
        // ショット・チャージ処理 (ここを整理・統合しました)
        // =========================================================

        // 1. 押し始め (初期化)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressTime = Time.time;
            isCharging = true;
            
            // 0.6秒未満は無音(first)からスタート
            currentChargeType = DrinkSoundType.first;
            isSoundPlaying = false;
            audioSource.Stop();
        }

        // 2. 離した時 (発射)
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // 速度リセット
            if (isSpeedReduced)
            {
                moveSpeed = originalMoveSpeed;
                isSpeedReduced = false;
            }

            isCharging = false;
            moveSpeed = originalMoveSpeed;
            holdTime = (Time.time - spacePressTime) * chargeTimeMultiplier;

            // チャージ音を停止
            audioSource.Stop();
            audioSource.loop = false;
            isSoundPlaying = false;

            // 発射処理
            DrinkSoundType shotSoundToPlay = DrinkSoundType.first_shot; // 仮
            bool shouldPlayShotSound = false; // 連打対策フラグ

            if (isBlackBoosted)
            {
                shouldPlayShotSound = true; // 黒エナドリ中は連打でも音を鳴らす
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
                // 通常時
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
                    shouldPlayShotSound = true; // 0.6秒以上なので鳴らす
                }
                else {
                    // 連打 (0.6秒未満)
                    ThrowBox(false, false, new Vector2(1f, 0.25f));
                    shotSoundToPlay = DrinkSoundType.first_shot;
                    shouldPlayShotSound = false; // ★鳴らさない
                }
            }

            // フラグが立っている場合のみショット音再生
            if (shouldPlayShotSound)
            {
                audioSource.PlayOneShot(enegyDrinkSound[(int)shotSoundToPlay]);
            }
        }

        // 3. チャージ中 (音の切り替え管理)
        if (isCharging)
        {
            holdTime = (Time.time - spacePressTime) * chargeTimeMultiplier;
            DrinkSoundType targetChargeType;

            // 時間経過によるランク判定
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
                targetChargeType = DrinkSoundType.second; // ここから音が鳴り始める
            }
            else
            {
                targetChargeType = DrinkSoundType.first; // 0.6秒未満は無音
            }

            // ランクが変わった瞬間のみ処理
            if (currentChargeType != targetChargeType)
            {
                audioSource.Stop();
                currentChargeType = targetChargeType;

                // 無音ランク(first)以外なら音を鳴らす
                if (targetChargeType != DrinkSoundType.first)
                {
                    // ループ音再生
                    audioSource.clip = enegyDrinkSound[(int)targetChargeType];
                    audioSource.loop = true;
                    audioSource.Play();

                    // 完了通知音(ショット音)を重ねて再生
                    int shotIndex = (int)targetChargeType + 3;
                    if (shotIndex < enegyDrinkSound.Length)
                    {
                        audioSource.PlayOneShot(enegyDrinkSound[shotIndex]);
                    }
                }
                isSoundPlaying = true;
            }
        }
    }

    // --- ThrowBoxなどの関数は変更なし ---

    void ThrowBox(bool explosive, bool isBox, Vector2 throwDir)
    {
        GameObject prefabToThrow = isBox ? boxPrefab : canprefab;
        if (prefabToThrow == null || throwPoint == null) return;

        GameObject obj = Instantiate(prefabToThrow, throwPoint.position, Quaternion.identity);
        audioSource.PlayOneShot(throwSound, throwSoundVolume);

        // CanBehaviorが存在する場合の処理 (元のコードにあったため維持)
        // ※CanBehaviorクラスがなければここはエラーになるので、必要に応じてコメントアウトしてください
        /*
        CanBehavior can = obj.GetComponent<CanBehavior>();
        if (can != null)
        {
            can.explosive = explosive;
        }
        */

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
            sr.color = Color.red;                 // 赤
            yield return new WaitForSeconds(damageflashDuration);

            sr.color = Color.white;               // 元に戻す
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
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // HPがmaxを超えないように
        UpdateHPBar(); // HPバーを更新
        audioSource.PlayOneShot(healSound, healSoundVolume);
        Debug.Log($"回復！ 現在のHP: {currentHealth}");
    }

    private IEnumerator SpeedBoostCoroutine(float boostAmount, float duration)
    {
        isSpeedBoosted = true;
        moveSpeed *= boostAmount; // スピードを上げる
        Debug.Log("スピードアップ！");

        yield return new WaitForSeconds(duration); // 指定時間待つ

        moveSpeed = originalMoveSpeed; // 元に戻す
        isSpeedBoosted = false;
        Debug.Log("元のスピードに戻った");
    }

    IEnumerator InvincibleCoroutine() // 無敵時間を管理するコルーチン (part5で追加)
    {
        yield return new WaitForSeconds(hitIntervalSec); // 指定した秒数待機
        isInvincible = false; // 無敵状態を解除
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
        chargeTimeMultiplier = chargeMultiplier; // チャージ時間を短縮（例: 0.5f）
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
        // groundCheckが設定されている場合のみ実行
        if (groundCheck != null)
        {
            // isGroundedがtrueなら緑、falseなら赤色に設定
            Gizmos.color = isGrounded ? Color.green : Color.red;

            // UpdateのOverlapCircleと全く同じ位置・半径で円を描画する
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}