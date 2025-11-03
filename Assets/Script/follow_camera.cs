using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // 追従する対象（プレイヤー）
    public float smoothSpeed = 5f; // カメラの追従スピード
    public Vector3 offset;         // プレイヤーからのオフセット
    public float leftLimit = -10f; // カメラが行ける左端の座標（ここを調整！）

    void LateUpdate()
    {
        if (target == null) return;

        // 目標位置を計算
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // カメラ位置を取得
        float targetX = smoothedPosition.x;

        // 左端制限を適用
        if (targetX < leftLimit)
            targetX = leftLimit;

        // カメラを移動（Zはそのまま）
        transform.position = new Vector3(targetX, smoothedPosition.y, transform.position.z);
    }
}
