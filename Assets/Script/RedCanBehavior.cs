using UnityEngine;

public class RedCanBehavior : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("スピードアップ！");
                player.SpeedBoost(1.5f, 3f); // ← 3秒間スピード1.5倍
                player.Heal(30); // HPを20回復（お好みで変更）
            }

            Debug.Log("赤エナドリを取得！");
            Destroy(gameObject);
        }
    }
}
