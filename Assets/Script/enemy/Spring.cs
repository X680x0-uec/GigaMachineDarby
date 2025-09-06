using UnityEngine;
using UnityEngine.UI;
using System;

public class Spring : MonoBehaviour
{

    [Header("移動設定")]
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float JumpForce = 7f;
    [SerializeField] private float MiniJumpForce = 4f;
    [SerializeField] private int JumpCount = 1;
    [SerializeField] private int MaxJump = 3;
    [SerializeField] bool target = true;
    [SerializeField] bool NewModel = true;
    private bool detect = false;
    /*target == trueならば右にはねながら一方通行で動く
    target == true かつ NewModel == falseなら検知範囲内にプレイヤーがいるときだけ追いかける
    target == true かつ NewModel == trueなら一度検知範囲内でプレイヤーを検知したら追いかけ続ける
    の3形態を用意しています*/

    private Rigidbody2D rb;
    private Enemy enemy; //索敵やステータスを管理するスクリプト

    private void Start()
    {
        // Rigidbody2D と Animator と を取得
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        if (target && !NewModel)
        {
            MoveSpeed *= 2;
        }
    }

    //オブジェクトと接触した際targetの状態によって挙動を変更
    void OnTriggerEnter2D(Collider2D other)
    {
        if (target && !NewModel)
        {
            TargetSpring();
        }
        else if (target && NewModel)
        {
            NewModelSpring();
        }
        else if(!target)
        {
            UnTargetSpring();
        }
    }

    //右に一方通行で動く場合
    private void UnTargetSpring()
    {
        if (JumpCount < MaxJump)
        {
            rb.linearVelocity = new Vector2(-MoveSpeed, JumpForce);
            JumpCount++;
        }
        else if (JumpCount == MaxJump)
        {
            rb.linearVelocity = new Vector2(-MoveSpeed, 2 * JumpForce);
            JumpCount = 1;
        }
    }

    //プレイヤーを追いかける場合
    private void TargetSpring()
    {
        //プレイヤーが索敵範囲外の時その場ではねる
        if (!enemy.IsDetected && enemy.DetectedPlayer == null)
        {
            rb.linearVelocity = new Vector2(0, MiniJumpForce);
            enemy.detectionRadius = 5f;
        }
        //プレイヤーが索敵範囲の時追いかける
        else if (enemy.IsDetected && enemy.DetectedPlayer != null)
        {
            enemy.detectionRadius = 15f;
            if (JumpCount < MaxJump)
            {
                MoveTowardsPlayer(JumpForce);
                JumpCount++;
            }
            else
            {
                MoveTowardsPlayer(2 * JumpForce);
                JumpCount = 1;
            }
        }
    }

    private void NewModelSpring()
    {
        if (!enemy.IsDetected && enemy.DetectedPlayer == null && !detect)
        {
            rb.linearVelocity = new Vector2(0, MiniJumpForce);
        }
        else if (enemy.IsDetected && enemy.DetectedPlayer != null && !detect)
        {
            detect = true;
            enemy.detectionRadius = Mathf.Infinity;
            MoveTowardsPlayer(JumpForce);
            JumpCount++;
        }
        else if (detect)
        {
            if (JumpCount < MaxJump)
            {
                MoveTowardsPlayer(JumpForce);
                JumpCount++;
            }
            else
            {
                MoveTowardsPlayer(2 * JumpForce);
                JumpCount = 1;
            }
        }
    }

    private void MoveTowardsPlayer(float JumpForce)
    {
        // 相対座標→正規化
        float direction = (enemy.DetectedPlayer.transform.position.x - transform.position.x);
        direction = direction < 0 ? -1 : 1;

        rb.linearVelocity = new Vector2(direction * MoveSpeed, JumpForce);

        // 移動方向に応じてスプライトの向きを変える
        FlipSprite(rb.linearVelocity.x);
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
}
