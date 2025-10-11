// DeathHandler.cs 【最終確認コード】

using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [Header("分裂設定")]
    public GameObject childPrefab;
    public int numberOfChildren = 2;
    [Tooltip("このスケール以下なら停止")]
    public float minScale = 0.5f;

    [Header("子のステータス変化")]
    public float sizeMultiplier = 0.6f;
    public float healthMultiplier = 0.5f;

    [Header("エフェクト設定")]
    public float spawnForce = 5f;

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public bool TryHandleDeath(Vector2 knockbackDirection)
    {
        float currentAbsScaleX = Mathf.Abs(transform.localScale.x);

        if (currentAbsScaleX <= minScale || childPrefab == null)
        {
            return false;
        }

        for (int i = 0; i < numberOfChildren; i++)
        {
            GameObject childObj = Instantiate(childPrefab, transform.position, Quaternion.identity);


            Vector2 newScale = new Vector2(currentAbsScaleX * sizeMultiplier, Mathf.Abs(transform.localScale.y) * sizeMultiplier);
            childObj.transform.localScale = newScale;

            Enemy childEnemy = childObj.GetComponent<Enemy>();
            if (childEnemy != null && enemy != null)
            {
                int newChildMaxHP = Mathf.Max(1, (int)(enemy.maxHP * healthMultiplier));
                childEnemy.InitializeForSplit(newChildMaxHP);
            }

            // 子を飛び出させるエフェクト
            Rigidbody2D childRb = childObj.GetComponent<Rigidbody2D>();
            if (childRb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                childRb.AddForce(randomDirection * spawnForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
        return true;
    }
}