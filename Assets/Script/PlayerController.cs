using UnityEngine;
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

        // ジャンプ（上キー）
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHPBar();

        if (currentHealth <= 0)
        {
            Die();
        }
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

    private void OnDrawGizmosSelected()
    {
        // 地面判定の確認用
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
