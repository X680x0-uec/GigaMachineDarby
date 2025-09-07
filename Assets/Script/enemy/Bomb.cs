using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 8f; // 敵の移動速度

    // 参照するコンポーネント
    private Rigidbody2D rb;
    private Enemy enemy; //索敵やステータスを管理するスクリプト
    private enemy_contact EnemyContact; //接触によるダメージの判定と値を管理するスクリプト

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        EnemyContact = GetComponent<enemy_contact>();
        EnemyContact.damage = 50; //配置するたびにインスペクターから変更しなくて済むための処理
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.TakeDamage(100); //プレイヤーに接触するとこのオブジェクトが死亡
        }
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y); //横向きに移動。
                                                                          //このスクリプトだと地面に接触していないと使えないので
                                                                          //空中でも使えるように後日修正予定
    }
}
