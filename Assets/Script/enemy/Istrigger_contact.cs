using UnityEngine;

public class Istrigger_contact : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage, Vector2.zero);

            Debug.Log("Player‚Éƒ_ƒ[ƒW‚ğ—^‚¦‚Ü‚µ‚½");
        }

    }
}

