using UnityEngine;

public class EnemyBoxBehavior : MonoBehaviour
{
    public bool isExplosive = false;
    public GameObject explosionPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExplosive)
        {
            if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("Player"))
            {

                if (explosionPrefab != null)
                {
                    GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    Destroy(explosion, 1f);
                }
                else
                {
                    Debug.LogWarning("explosionPrefabがInspectorで設定されていません");
                }

                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("Player"))
        {
            Invoke("Destroy", 0.1f);
        }
    }
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
