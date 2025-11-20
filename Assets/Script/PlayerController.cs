using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{    
    [Header("スプライト設定")]
    public Sprite standingRightSprite;
    public Sprite standingLeftSprite; 
    public Sprite runRightSprite; 
    public Sprite runLeftSprite; 

    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    private bool isSpeedBoosted = false; // スピードアップ中かどうか
    private float originalMoveSpeed;     // 元のスピードを保存

    //[SerializeField] private int jumpCount = 0;
    //[SerializeField] private int maxJumpCount = 2;

    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float hitIntervalSec = 0.3f;


    [Header("ショット設定")]
    [SerializeField] private bool isBlackBoosted = false;
    private float chargeTimeMultiplier = 1f; // チャージ時間短縮用（1f=通常、0.5fなら半分）

    public GameObject canprefab;
    public GameObject boxPrefab;
    public Transform throwPoint;
    public float throwForce = 10f;
    private float spacePressTime;     // スペースを押し始めた時間
    private float holdTime;
    private bool isCharging = false;
    private bool isSpeedReduced = false;
    public AudioClip throwSound; 
    [SerializeField, Range(0f, 1f)]
    private float throwSoundVolume = 1f;
[Header("チャージ音（ループ）")]
public AudioClip chargeLevel0Loop;
public AudioClip chargeLevel1Loop;
public AudioClip chargeLevel2Loop;
public AudioClip chargeLevel3Loop;

[Header("チャージ音（切り替え時）")]
public AudioClip chargeLevel1Start;
public AudioClip chargeLevel2Start;
public AudioClip chargeLevel3Start;
private int currentChargeLevel = 0;
private AudioSource chargeAudioSource;


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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        UpdateHPBar();
        originalMoveSpeed = moveSpeed;
        audioSource = gameObject.AddComponent<AudioSource>();
        sr.sprite = standingRightSprite; // 初期状態を右向きに
    chargeAudioSource = gameObject.AddComponent<AudioSource>();
    chargeAudioSource.loop = true;
    }

    void Update()//アップデート
    {
        if (isDead) return;

        // 左右入力のみ
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 向き反転
        // 向きの変更
        if (moveInput > 0)
        {
            facingRight = true;
        }
        else if (moveInput < 0)
        {
            facingRight = false;
        }

        // 状態ごとの画像切り替え
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            // 移動中
            sr.sprite = facingRight ? runRightSprite : runLeftSprite;
        }
        else
        {
            // 停止中
            sr.sprite = facingRight ? standingRightSprite : standingLeftSprite;
        }

        // 地面判定
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

         if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)// 上矢印キーが押されたとき
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);// 上に移動
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))// 上矢印キーが離されたとき
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);// まだ上昇中ならジャンプをカット
            }
        }

if (Input.GetKeyDown(KeyCode.Space))
{
    spacePressTime = Time.time;
    isCharging = true;
    currentChargeLevel = 0;

    // 0段階ループ音開始
    chargeAudioSource.clip = chargeLevel0Loop;
    chargeAudioSource.Play();
}

void ChangeChargeSound(int level)
{
    switch (level)
    {
        case 0:
            chargeAudioSource.clip = chargeLevel0Loop;
            chargeAudioSource.Play();
            break;

        case 1:
            chargeAudioSource.clip = chargeLevel1Loop;
            chargeAudioSource.Play();
            audioSource.PlayOneShot(chargeLevel1Start);
            break;

        case 2:
            chargeAudioSource.clip = chargeLevel2Loop;
            chargeAudioSource.Play();
            audioSource.PlayOneShot(chargeLevel2Start);
            break;

        case 3:
            chargeAudioSource.clip = chargeLevel3Loop;
            chargeAudioSource.Play();
            audioSource.PlayOneShot(chargeLevel3Start);
            break;
    }
}


if (Input.GetKeyUp(KeyCode.Space))
{
    isCharging = false;

    if (chargeAudioSource.isPlaying)
        chargeAudioSource.Stop();

    // ここから投げる処理
    holdTime = (Time.time - spacePressTime) * chargeTimeMultiplier;
    if (chargeAudioSource.isPlaying)
        chargeAudioSource.Stop();
            

            if (isBlackBoosted)
            {
                // 黒エナドリ中の順番
                if (holdTime >= 2.6f)
                {
                    ThrowBox(true, true, new Vector2(1f, 0.25f)); // 箱
                }
                else if (holdTime >= 1.6f)
                {
                    // 缶3つ
                    ThrowBox(true, false, new Vector2(1f, 0.8f));
                    ThrowBox(true, false, new Vector2(1f, 0.1f));
                    ThrowBox(true, false, new Vector2(1f, -0.05f));
                }
                else
                {
                    // 爆発缶1つ
                    ThrowBox(true, false, new Vector2(1f, 0.25f));
                }
            }

            else
            {
                if (holdTime >= 2.6f)
                {
                    ThrowBox(true, true, new Vector2(1f, 0.25f));
                }
                else if (holdTime >= 1.6f)
                {
                    // 中押し → 缶3つを投げる（上・中・下）
                    ThrowBox(true, false, new Vector2(1f, 0.8f));   // 上方向
                    ThrowBox(true, false, new Vector2(1f, 0.1f));     // 真横
                    ThrowBox(true, false, new Vector2(1f, -0.05f));  // 下方向
                }
                else if (holdTime >= 0.6f)
                {
                    // 少し押し → 缶1つを真横に飛ばす
                    ThrowBox(true, false, new Vector2(1f, 0.25f));
                }
                else
                {
                    // 短押し → 爆発しない缶を真横に飛ばす
                    ThrowBox(false, false, new Vector2(1f, 0.25f));
                }
            }
        }
if (isCharging)
{
    holdTime = (Time.time - spacePressTime) * chargeTimeMultiplier;

    int newLevel = 0;
    if (holdTime >= 2.6f) newLevel = 3;
    else if (holdTime >= 1.6f) newLevel = 2;
    else if (holdTime >= 0.6f) newLevel = 1;

    // レベルが変わった時だけ音を切り替える
    if (newLevel != currentChargeLevel)
    {
        currentChargeLevel = newLevel;
        ChangeChargeSound(newLevel);
    }
}

    }

void ThrowBox(bool explosive, bool isBox, Vector2 throwDir)
{
    GameObject prefabToThrow = isBox ? boxPrefab : canprefab;
    if (prefabToThrow == null || throwPoint == null) return;

    GameObject obj = Instantiate(prefabToThrow, throwPoint.position, Quaternion.identity);
    audioSource.PlayOneShot(throwSound, throwSoundVolume);

    if (!isBox)
    {
        CanBehavior can = obj.GetComponent<CanBehavior>();
        if (can != null)
        {
            can.explosive = explosive;
        }
    }

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
