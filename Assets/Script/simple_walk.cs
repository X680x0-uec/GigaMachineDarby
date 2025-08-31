using UnityEngine;

// Rigidbody2DとEnemyスクリプトが必須であることを示す
public class EnemyMovement : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 2.5f; // 敵の移動速度

    // 参照するコンポーネント
    private Rigidbody2D rb;
    private Enemy enemy; //索敵やステータスを管理するスクリプト

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
    }
    private void FixedUpdate()
    {
        //Enemyスクリプトがプレイヤーを検知しているか確認
        if (enemy.IsDetected && enemy.DetectedPlayer != null)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMoving();
        }
    }

    private void MoveTowardsPlayer()
    {
        // 相対座標→正規化
        float direction = (enemy.DetectedPlayer.transform.position.x - transform.position.x);
        direction = direction < 0 ? -1 : 1;

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // 移動方向に応じてスプライトの向きを変える
        FlipSprite(rb.linearVelocity.x);
    }

    private void StopMoving()
    {
        // 速度を0にして停止させる
        rb.linearVelocity = Vector2.zero;
    }

    private void FlipSprite(float horizontalVelocity)
    {
        // 速度が右向き（0より大きい）ならスプライトはそのまま
        if (horizontalVelocity > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // 速度が左向き（0より小さい）ならスプライトを反転させる
        else if (horizontalVelocity < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        // ほぼ停止している場合は何もしない
    }

    public void Jump(float JumpForce)
    {
        // 相対座標→正規化
        float direction = (enemy.DetectedPlayer.transform.position.x - transform.position.x);
        direction = direction < 0 ? -1 : 1;

        rb.linearVelocity = new Vector2(direction * moveSpeed, JumpForce);

        // 移動方向に応じてスプライトの向きを変える
        FlipSprite(rb.linearVelocity.x);
    }
}