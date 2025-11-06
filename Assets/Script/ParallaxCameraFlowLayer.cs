// 視差スクロール用のレイヤーに設定するスクリプト
using UnityEngine;

public class ParallaxCameraFlowLayer : MonoBehaviour
{
    // 追従対象のカメラ
    [SerializeField] Transform _cameraTransfrom;
    // カメラに追従する程度(1: カメラと同じ移動量 0: 移動しない)
    [SerializeField] float _followFactor;

    Vector3 _previousCameraPos;

    private void Update()
    {
        Vector3 currentPos = _cameraTransfrom.position;
        var deltaPos = currentPos - _previousCameraPos;
        _previousCameraPos = currentPos;
        var calcedPos = deltaPos * _followFactor;
        transform.AddLocalPos((Vector2)calcedPos);
    }
}

// ユーティリティ
public static class ParallaxCameraFlowLayerExtensions
{
    public static void AddLocalPos(this Transform self, in Vector2 pos)
    {
        Vector3 vec = self.localPosition;
        vec.x += pos.x;
        self.localPosition = vec;
    }
}
