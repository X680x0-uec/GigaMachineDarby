using UnityEngine;

public class MovingPlatformVertical : MonoBehaviour
{
    public float topY = 3f;     // 上の位置
    public float bottomY = 0f;  // 下の位置
    public float speed = 2f;    // 移動速度

    private Vector3 target;

    void Start()
    {
        // 最初は上に向かう
        target = new Vector3(transform.position.x, topY, transform.position.z);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        // 目的地についたら方向を反転
        if (Mathf.Abs(transform.position.y - target.y) < 0.1f)
        {
            if (target.y == topY)
                target = new Vector3(transform.position.x, bottomY, transform.position.z);
            else
                target = new Vector3(transform.position.x, topY, transform.position.z);
        }
    }
}
