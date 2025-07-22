using UnityEngine;

public class EnemyHPBarFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1.5f, 0);  // ���̏�̃I�t�Z�b�g

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

            // Z��]�������ɌŒ�i2D�ł͂����OK�j
            transform.rotation = Quaternion.identity;
        }
    }
}
