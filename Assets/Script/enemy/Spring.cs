using UnityEngine;
using UnityEngine.UI;
using System;

public class Spring : MonoBehaviour
{
    [Header("体力ステータス")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("HPバー UI（Fill Image）")]
    [SerializeField] private Slider HPbar;

    // イベント（必要に応じて活用）
    public event Action OnDamaged;
    public event Action OnHealed;
    public event Action OnDied;

    [Header("ノックバック設定")]
    public float knockbackForce = 5f;
    public float deathDelay = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    [Header("移動設定")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jump = 5f;
    [SerializeField] private int JumpCount = 0;
    [SerializeField] private int MaxJump = 3;

    public bool IsDead => currentHealth <= 0;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHPBar();

        // Rigidbody2D と Animator を取得
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        {
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocityY);
        }
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnDamaged?.Invoke();
        UpdateHPBar();

        if (currentHealth <= 0)
        {
            OnDied?.Invoke();

            Die(Vector2.zero);
        }
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealed?.Invoke();
        UpdateHPBar();
    }

    /// <summary>
    /// HPバー更新
    /// </summary>
    private void UpdateHPBar()
    {
        if (HPbar != null)
        {
            HPbar.value = (float)currentHealth / maxHealth;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: hpBar が設定されていません！");
        }
    }

    private void Die(Vector2 direction)
    {
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // linearVelocityは存在しないため修正
            rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        /*
        if (animator != null)
        {
            animator.SetTrigger("Die"); // 死亡アニメーション（任意）
        }
        */

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, deathDelay);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (JumpCount != MaxJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jump);
            JumpCount++;
        }else if(JumpCount == MaxJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 2*jump);
            JumpCount = 0;
        }
    }
}
