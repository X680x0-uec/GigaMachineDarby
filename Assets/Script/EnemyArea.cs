using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    [Header("カメラ追従スクリプト")]
    public CameraFollow cameraFollow;

    [Header("このエリア内の敵たち")]
    public GameObject[] enemies;

    [Header("カメラを固定する位置X/Y")]
    public float fixedCameraX = 0f;
    public float fixedCameraY = 0f;

    [Header("エリアの左右に置く壁のプレハブ")]
    public GameObject wallPrefab;

    [Header("壁の生成位置オフセット")]
    public float wallOffsetX = 8f;  // カメラ中心から左右どれくらい離すか

    [Header("壁の高さ（地面からのオフセット）")]
    public float wallYOffset = 1.5f;

    private bool isCameraLocked = false;
    private GameObject leftWall;
    private GameObject rightWall;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCameraLocked)
        {
            isCameraLocked = true;

            // カメラ追従をオフ
            cameraFollow.SetFollowEnabled(false);

            // カメラ位置を固定
            Vector3 camPos = cameraFollow.transform.position;
            camPos.x = fixedCameraX;
            camPos.y = fixedCameraY;
            cameraFollow.transform.position = camPos;

            //  壁を生成
            SpawnWalls();
        }
    }

    private void Update()
    {
        if (isCameraLocked && AllEnemiesDefeated())
        {
            cameraFollow.SetFollowEnabled(true);
            isCameraLocked = false;

            //  壁を削除
            DestroyWalls();
            //　オブジェクトを削除
            Destroy(gameObject);
        }
    }

    private bool AllEnemiesDefeated()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null) return false;
        }
        return true;
    }

    private void SpawnWalls()
    {
        if (wallPrefab == null)
        {
            Debug.LogWarning("wallPrefab が設定されていません！");
            return;
        }

        Vector3 camPos = cameraFollow.transform.position;

        // 左右に壁を生成
        leftWall = Instantiate(wallPrefab, new Vector3(camPos.x - wallOffsetX, camPos.y, 0f), Quaternion.identity);
        rightWall = Instantiate(wallPrefab, new Vector3(camPos.x + wallOffsetX, camPos.y, 0f), Quaternion.identity);
    }

    private void DestroyWalls()
    {
        if (leftWall != null) Destroy(leftWall);
        if (rightWall != null) Destroy(rightWall);
        
    }
}
