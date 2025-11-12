using UnityEngine;

public class BoxBehavior : MonoBehaviour
{
    public bool isExplosive = false;
    public GameObject explosionPrefab;
    public float explosionRadius = 2.5f; 
    public int explosionDamage = 20;  
    public LayerMask damageTargetLayer;
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
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                foreach (var hit in hits)
                {
                    // 自分は無視
                    if (hit.gameObject == gameObject) continue;

                    // プレイヤーならスキップ（自爆防止）
                    if (hit.GetComponent<PlayerController>() != null) continue;

                    // 敵スクリプトを持っていればダメージを与える
                    Enemy enemy = hit.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(explosionDamage);
                        Debug.Log($"💥 {enemy.name} に {explosionDamage} ダメージ！");
                    }
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
    private void OnDrawGizmosSelected()
    {
        // Sceneビューで選択中に、爆発範囲を赤い円で表示
        Gizmos.color = new Color(1, 0, 0, 0.4f); // 半透明の赤
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
