using UnityEngine;

public class HPBarRotationFix : MonoBehaviour
{
    private Transform parentTransform;
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Inspectorから調整可能

    void Start()
    {
        // 親オブジェクトのTransformを取得
        parentTransform = transform.parent;
    }

    void LateUpdate()
    {
        // 1. 位置の追従
        if (parentTransform != null)
        {
            // 親オブジェクトの位置にオフセットを加えた位置に設定
            transform.position = parentTransform.position + offset;
        }

        // 2. 回転の固定
        // 常にまっすぐな状態（回転ゼロ）を維持
        transform.rotation = Quaternion.identity;
    }
}