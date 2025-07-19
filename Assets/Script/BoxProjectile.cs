using UnityEngine;

public class BoxProjectile : MonoBehaviour
{
    public int damage = 20;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            // 2引数版ではなく1引数で呼ぶ
            enemy.TakeDamage(damage);

            Debug.Log("Enemyにダメージを与えました");
        }

    }
}
