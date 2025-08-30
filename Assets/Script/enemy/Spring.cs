using UnityEngine;
using UnityEngine.UI;
using System;

public class Spring : MonoBehaviour
{
    [Header("�̗̓X�e�[�^�X")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("HP�o�[ UI�iFill Image�j")]
    [SerializeField] private Slider HPbar;

    // �C�x���g�i�K�v�ɉ����Ċ��p�j
    public event Action OnDamaged;
    public event Action OnHealed;
    public event Action OnDied;

    [Header("�m�b�N�o�b�N�ݒ�")]
    public float knockbackForce = 5f;
    public float deathDelay = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    [Header("�ړ��ݒ�")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jump = 5f;
    [SerializeField] private int JumpCount = 0;
    [SerializeField] private int MaxJump = 3;

    public bool IsDead => currentHealth <= 0;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHPBar();

        // Rigidbody2D �� Animator ���擾
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
    /// �_���[�W���󂯂�
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
    /// �񕜏���
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
    /// HP�o�[�X�V
    /// </summary>
    private void UpdateHPBar()
    {
        if (HPbar != null)
        {
            HPbar.value = (float)currentHealth / maxHealth;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: hpBar ���ݒ肳��Ă��܂���I");
        }
    }

    private void Die(Vector2 direction)
    {
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // linearVelocity�͑��݂��Ȃ����ߏC��
            rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        /*
        if (animator != null)
        {
            animator.SetTrigger("Die"); // ���S�A�j���[�V�����i�C�Ӂj
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
