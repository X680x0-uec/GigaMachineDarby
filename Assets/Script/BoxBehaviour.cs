using UnityEngine;

public class BoxBehavior : MonoBehaviour
{
    public bool isExplosive = false;
    public GameObject explosionPrefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExplosive)
        {
            if (collision.gameObject.CompareTag("floor") || collision.gameObject.CompareTag("Enemy"))
            {

                if (explosionPrefab != null)
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("explosionPrefabがInspectorで設定されていません");
                }

                Destroy(gameObject);
            }
        }
    }
}
