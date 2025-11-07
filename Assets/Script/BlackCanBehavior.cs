using UnityEngine;

public class BlackCanBehavior : MonoBehaviour
{
    public float boostDuration = 5f;         // 黒エナドリ効果時間
    public float chargeTimeMultiplier = 0.5f; // チャージ時間短縮（0.5で半分に）

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("黒エナドリを取得！");
                player.ActivateBlackBoost(boostDuration, chargeTimeMultiplier);
            }

            Destroy(gameObject); // エナドリを消す
        }
    }
}
