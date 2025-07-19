using UnityEngine;

public class EnemyHPBarFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1.5f, 0);  // 頭の上のオフセット

    private Transform target;

    void Start()
    {
        target = transform.parent;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;

            // Z回転させずに固定（2DではこれでOK）
            transform.rotation = Quaternion.identity;
        }
    }
}
