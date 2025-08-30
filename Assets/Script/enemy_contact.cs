using UnityEngine;

public class enemy_contact : MonoBehaviour
{
    public int damage = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage, Vector2.zero);

            Debug.Log("Playerにダメージを与えました");
        }

    }
}

