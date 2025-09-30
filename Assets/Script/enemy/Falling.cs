using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Falling : MonoBehaviour
{
    // 参照するコンポーネント
    private Rigidbody2D rb;
    private Enemy enemy; //索敵やステータスを管理するスクリプト
    private enemy_contact EnemyContact; //接触によるダメージの判定と値を管理するスクリプト

    bool falled = false; 
    bool falling = false;
    [SerializeField] private float falling_speed = 6.0f;
    [SerializeField] private float rising_speed = 3.0f;
    float start_x;
    float start_y;
    private Vector2 currentVelocity; //座標の変更用
    private Vector2 target;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Rigidbody2D と Animator と を取得
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        rb.bodyType = RigidbodyType2D.Static; //ほかオブジェクトと衝突した際にfallingが押し出されないようにする
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        float start_x = transform.position.x;
        float start_y = transform.position.y;
        target.x = start_x;
        target.y = start_y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!falled)
        {
            if (enemy.IsDetected && enemy.DetectedPlayer != null)
            {
                float falling_x = transform.position.x;
                float falling_y = transform.position.y;
                float player_x = enemy.DetectedPlayer.transform.position.x;
                float player_y = enemy.DetectedPlayer.transform.position.y;
                if (System.Math.Abs(falling_x - player_x) < 1.5 && falling_y > player_y)
                {
                    Debug.Log("検知");
                    falling = true;
                }
                if (falling)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    currentVelocity.y = falling_speed;
                    transform.position -= (Vector3)currentVelocity * Time.deltaTime;
                }
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                rising_speed * Time.deltaTime);

            if ((Vector2)transform.position == target)
            {
                falled = false;
                rb.bodyType = RigidbodyType2D.Static;
                transform.position = transform.position;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("floor"))
        {
            falling = false;
            falled = true;
        }
    }
}
