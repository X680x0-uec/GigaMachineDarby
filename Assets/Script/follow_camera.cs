using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // 追従する対象（プレイヤー）
    public float smoothSpeed = 5f; // カメラの追従スピード
    public Vector3 offset;         // プレイヤーからのオフセット
    public float leftLimit = -10f; // カメラが行ける左端の座標
    public float rightLimit = 10f; // カメラが行ける右端の座標

    private bool followEnabled = true; // ← カメラ追従のオン・オフ切り替え用

    void LateUpdate()
    {
        if (!followEnabled || target == null) return;

        // 目標位置を計算
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // カメラ位置を取得
        float targetX = smoothedPosition.x;

        // 左端制限を適用
        if (targetX < leftLimit)
            targetX = leftLimit;

        // 右端制限を適用
        if (targetX > rightLimit)
            targetX = rightLimit;

        // カメラを移動（Zはそのまま）
        transform.position = new Vector3(targetX, smoothedPosition.y, transform.position.z);
    }

    
    public void SetFollowEnabled(bool enabled)
    {
        followEnabled = enabled;
    }
}


