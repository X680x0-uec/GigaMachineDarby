using UnityEngine;

public class enemy_contact : MonoBehaviour
{
    public int damage = 10;
    public float damageInterval = 1.0f; 
    private float nextDamageTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time >= nextDamageTime)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage, Vector2.zero);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            nextDamageTime = 0; 
        }
    }
}