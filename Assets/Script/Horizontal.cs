using UnityEngine;

public class MovingPlatformHorizontal : MonoBehaviour
{
    public float leftX = -2f;   // 左端
    public float rightX = 2f;   // 右端
    public float speed = 2f;    // 速さ

    private Vector3 target;

    void Start()
    {
        // 最初は右方向へ
        target = new Vector3(rightX, transform.position.y, transform.position.z);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        // 端まで来たらターゲットを反転
        if (Mathf.Abs(transform.position.x - target.x) < 0.1f)
        {
            if (target.x == rightX)
                target = new Vector3(leftX, transform.position.y, transform.position.z);
            else
                target = new Vector3(rightX, transform.position.y, transform.position.z);
        }
    }

    // ▼ プレイヤーを床にくっつける処理（上下と同じ）
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
