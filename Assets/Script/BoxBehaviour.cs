using UnityEngine;

public class BoxBehavior : MonoBehaviour
{
    public bool isExplosive = false;
    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    [SerializeField, Range(0f, 1f)]
    private float explosionSoundVolume = 1f;
    private AudioSource audioSource;
    private void Start()
    {
        // AudioSourceをこのオブジェクトにアタッチ
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (isExplosive)
        {
            if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("Enemy"))
            {

                if (explosionPrefab != null)
                {
                    GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    audioSource.PlayOneShot(explosionSound, explosionSoundVolume);
                    Destroy(explosion, 1f);
                }
                else
                {
                    Debug.LogWarning("explosionPrefabがInspectorで設定されていません");
                }
                Destroy(GetComponent<SpriteRenderer>());
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) Destroy(col);
                Destroy(gameObject, 5f);
            }
        }
        else if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
