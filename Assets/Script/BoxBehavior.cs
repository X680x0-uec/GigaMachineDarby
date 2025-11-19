using UnityEngine;

public class BoxBehavior : MonoBehaviour
{
    [Header("箱から出る缶設定")]
    public GameObject canPrefab;
    public int minCanCount = 8;
    public int maxCanCount = 10;
    public float spawnForce = 7f;

    private bool hasSpawnedCans = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 地面に当たったら缶を生成（爆発なし）
        if (!hasSpawnedCans && collision.gameObject.CompareTag("floor"))
        {
            hasSpawnedCans = true;
            SpawnCans();
            Destroy(gameObject);     // 箱は消す
        }
    }

    private void SpawnCans()
    {
        int canCount = Random.Range(minCanCount, maxCanCount + 1);

        for (int i = 0; i < canCount; i++)
        {
            GameObject can = Instantiate(canPrefab, transform.position, Quaternion.identity);

            // "箱からの缶" であることを伝える
            CanBehavior cb = can.GetComponent<CanBehavior>();
            if (cb != null)
            {
                cb.isFromEnergyBox = true;
            }

            // ランダムな方向に飛ばす
            Rigidbody2D rb = can.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDir = new Vector2(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0.3f, 1f)
                ).normalized;

                rb.AddForce(randomDir * spawnForce, ForceMode2D.Impulse);
            }
        }
    }
}
