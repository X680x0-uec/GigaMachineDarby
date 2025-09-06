using UnityEngine;

public class HPBarRotationFix : MonoBehaviour
{
    private Transform parentTransform;
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Inspector���璲���\

    void Start()
    {
        // �e�I�u�W�F�N�g��Transform���擾
        parentTransform = transform.parent;
    }

    void LateUpdate()
    {
        // 1. �ʒu�̒Ǐ]
        if (parentTransform != null)
        {
            // �e�I�u�W�F�N�g�̈ʒu�ɃI�t�Z�b�g���������ʒu�ɐݒ�
            transform.position = parentTransform.position + offset;
        }

        // 2. ��]�̌Œ�
        // ��ɂ܂������ȏ�ԁi��]�[���j���ێ�
        transform.rotation = Quaternion.identity;
    }
}