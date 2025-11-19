using UnityEngine;
using System.Collections;

public class CanBehavior : MonoBehaviour
{
    public bool explosive = true;  
    public bool isFromEnergyBox = false;
    public GameObject explosionPrefab;
    public float explosionRadius = 2.3f;
    public int explosionDamage = 20;

    public float autoExplosionTime = 1.5f;
    public AudioClip explosionSound;
    [SerializeField, Range(0f, 1f)]
    private float explosionSoundVolume = 1f;

    private bool exploded = false;
    private AudioSource audioSource;
    
    private void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = !isFromEnergyBox;   // 箱から出た缶だけ衝突判定ON
        }

        StartCoroutine(AutoExplode());
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    IEnumerator AutoExplode()
    {
        yield return new WaitForSeconds(autoExplosionTime);

        if (!exploded && explosive)
            Explode();
        else if (!explosive)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (exploded) return;

        if (other.GetComponent<Enemy>() != null)
        {
            if (explosive) Explode();
            else Destroy(gameObject);
            return;
        }

        if (!isFromEnergyBox)
        {
            if (other.CompareTag("floor") || other.CompareTag("wall"))
            {
                if (explosive) Explode();
                else Destroy(gameObject); 
            }
        }
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;
    if (explosionSound != null)
        AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionSoundVolume);
        if (explosionPrefab != null)
        {
            GameObject exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(exp, 3f);
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
