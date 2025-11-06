using UnityEngine;

public class HorizontalDynamicImageRotation : MonoBehaviour
{
    [SerializeField] Transform _cameraTransform;
    // ローテーションするときの1枚の画像の幅
    [SerializeField] float _imageWidth;

    Transform _parentLayer;
    Transform _transformCache;

    private void Awake()
    {
        _parentLayer = this.GetParent().transform;
        _transformCache = transform;

        if (!_cameraTransform)
        {
            Debug.LogWarning("カメラが設定されていません。", this);
            this.enabled = false;
        }
    }

    private void Update()
    {
        // 3枚でローテーションするのでカメラの描画範囲から1.5枚分ずれたらその方向に移動する
        var distance = _cameraTransform.position - _transformCache.position;
        if (Mathf.Abs(distance.x) > _imageWidth * 1.5f)
        {
            float amount = _imageWidth * 3 * (distance.x < 0 ? -1.0f : 1.0f);
            _transformCache.AddLocalPosX(amount);
        }
    }
}

// ユーティリティ
public static class HorizontalDynamicImageRotationExtensions
{
    // 親を取得
    public static GameObject GetParent(this Component self)
    {
        return self.transform.parent.gameObject;
    }
    // X方向に値を足す
    public static void AddLocalPosX(this Transform self, float x)
    {
        Vector3 vec3 = self.localPosition;
        vec3.x += x;
        self.localPosition = vec3;
    }
}
