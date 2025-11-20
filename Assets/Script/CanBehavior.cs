using UnityEngine;
using System.Collections;

public class CanBehavior : MonoBehaviour
{
    public bool isFromEnergyBox = false;

    public GameObject explosionPrefab;
    public float explosionRadius = 2.3f;
    public int explosionDamage = 20;

    public float autoExplosionTime = 1.5f;

    private bool exploded = false;

    private void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
    if (col != null)
    {
        col.isTrigger = !isFromEnergyBox; // 箱からの缶はTriggerをOFF
    }
        StartCoroutine(AutoExplode());
    }
    

    IEnumerator AutoExplode()
    {
        yield return new WaitForSeconds(autoExplosionTime);

        if (!exploded)
            Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    if (exploded) return;

    Enemy enemy = other.GetComponent<Enemy>();
    if (enemy != null)
    {
        Explode();
        return;
    }

    if (!isFromEnergyBox)
    {
        if (other.CompareTag("floor") || other.CompareTag("wall"))
        {
            Explode();
        }
    }
}


    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionPrefab != null)
        {
            GameObject exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(exp, 1f);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<PlayerController>() != null) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }
}
