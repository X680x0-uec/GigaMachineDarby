using UnityEngine;
using UnityEngine.UI;
using System;

public class Enemy : MonoBehaviour
{
    [Header("感知設定")]
    public float detectionRadius = 3f; // 検知距離
    public LayerMask playerLayer;      // プレイヤー用レイヤー

    public bool IsDetected { get; private set; }
    public GameObject DetectedPlayer { get; private set; }

    [Header("体力ステータス")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("HPバー UI")]
    public Image hpBarFill;

    public event Action OnDamaged;
    public event Action OnHealed;
    public event Action OnDied;

    [Header("ノックバック設定")]
    public float knockbackForce = 5f;
    public float deathDelay = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    public bool IsDead => currentHealth <= 0;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHPBar();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        DetectPlayer();
    }

    /// <summary>
    /// 距離判定でプレイヤーを感知
    /// </summary>
    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (hit != null)
        {
            if (!IsDetected)
                Debug.Log("プレイヤーを発見！");

            IsDetected = true;
            DetectedPlayer = hit.gameObject;
        }
        else
        {
            if (IsDetected)
                Debug.Log("プレイヤーを見失った...");

            IsDetected = false;
            DetectedPlayer = null;
        }
    }

    /// <summary>
    /// 検知範囲をScene上で可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsDetected ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

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

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealed?.Invoke();
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (hpBarFill != null)
        {
            hpBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: hpBarFill が設定されていません！");
        }
    }

    private void Die(Vector2 direction)
    {
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, deathDelay);
    }
}
