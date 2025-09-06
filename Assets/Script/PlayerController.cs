using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float hitIntervalSec = 0.3f;

    [Header("ショット設定")]
    public GameObject boxPrefab;
    public Transform throwPoint;
    public float throwForce = 10f;

    [Header("体力設定")]
    public int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private Slider HPbar;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateHPBar();
    }

    void Update()
    {
        if (isDead) return;

        // 左右入力のみ
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 向き反転
        if (moveInput != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveInput);
            transform.localScale = scale;
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
        // ショット（スペースキー）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowBox();
        }
    }

    void ThrowBox()
    {
        if (boxPrefab != null && throwPoint != null)
        {
            GameObject box = Instantiate(boxPrefab, throwPoint.position, Quaternion.identity);
            Rigidbody2D boxRb = box.GetComponent<Rigidbody2D>();

            if (boxRb != null)
            {
                float direction = transform.localScale.x >= 0 ? 1f : -1f;
                boxRb.AddForce(new Vector2(direction, 0.5f).normalized * throwForce, ForceMode2D.Impulse);
            }
        }
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead || isInvincible) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHPBar();
        rb.AddForce(transform.up * 400.0f);
        rb.AddForce(transform.right * -400.0f);

        isInvincible = true;
        StartCoroutine(InvincibleCoroutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator InvincibleCoroutine() // 無敵時間を管理するコルーチン
    {
        yield return new WaitForSeconds(hitIntervalSec); // 指定した秒数待機
        isInvincible = false; // 無敵状態を解除
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
