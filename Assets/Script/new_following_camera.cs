using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class new_following_camera : MonoBehaviour
{
    // --- Public/Serialized Fields (Inspectorで設定) ---
    public Transform target;            // 追従する対象（プレイヤー）
    public Vector3 offset;              // プレイヤーからのオフセット
    public float leftLimit = -10f;      // カメラが行ける左端の座標
    public float rightLimit = 10f;      // カメラが行ける右端の座標
    public float smoothSpeed = 5f;      // LateUpdateでの追従スピード (未使用だが互換性のため残す)
    public float first_leftlimit = 0f;
    public float first_rightlimit = 0f;
    [Header("境界と制限")]
    [SerializeField] private float limit = 10f; // horizontal状態での境界内での追従制限値

    // --- Private/State Fields ---
    private Coroutine currentCoroutine; // コルーチンの制御用
    [SerializeField] private axis nowstate;             // 現在のカメラの追従状態

    // 初期設定値
    [HideInInspector] public float first_position_y;
    [HideInInspector] public float first_leftLimit = 0f;
    [HideInInspector] public float first_rightLimit = 0f;

    // カメラの追従状態を定義
    public enum axis
    {
        vertical,                       // X追従, Y固定 (即時)
        horizontal,                     // X追従, Y追従 (即時)
        horizontal_to_vertical_transition, // 横→縦への遷移中 (0.1秒)
        vertical_to_horizontal_transition  // 縦→横への遷移中 (0.1秒)
    }

    // --- Unity Life Cycle ---

    private void Start()
    {
        first_leftLimit = leftLimit;
        first_rightLimit = rightLimit;

        if (target == null)
        {
            Debug.LogError("追従ターゲットが設定されていません。", this);
            enabled = false;
            return;
        }

        // 初期設定の保存
        nowstate = axis.vertical;
        first_position_y = transform.position.y;
        first_leftLimit = leftLimit;
        first_rightLimit = rightLimit;
    }

    void LateUpdate()
    {
        // コルーチン実行中（遷移中）は、通常追従ロジックをスキップ
        if (currentCoroutine != null)
        {
            return;
        }

        // 通常追従ロジック
        switch (nowstate)
        {
            case axis.vertical:
                HandleVerticalFollow();
                break;
            case axis.horizontal:
                HandleHorizontalFollow();
                break;
        }
    }

    // --- Private Follow Logic (即時追従) ---

    private void HandleVerticalFollow()
    {
        transform.position = new Vector3(target.transform.position.x - offset.x, first_position_y, transform.position.z);

        if (transform.position.x >= first_rightLimit)
        {
            transform.position = new Vector3(first_rightLimit, first_position_y, transform.position.z);
        }
        if (transform.position.x <= first_leftLimit)
        {
            transform.position = new Vector3(first_leftLimit, first_position_y, transform.position.z);
        }
    }

    private void HandleHorizontalFollow()
    {
        transform.position = new Vector3(target.transform.position.x - offset.x, target.transform.position.y - offset.y, transform.position.z);

        if (rightLimit - transform.position.x <= limit)
        {
            transform.position = new Vector3(rightLimit - limit, target.transform.position.y - offset.y, transform.position.z);
        }
        if (transform.position.x - leftLimit <= limit)
        {
            transform.position = new Vector3(leftLimit + limit, target.transform.position.y - offset.y, transform.position.z);
        }
    }

    // --- Public Transition Methods (外部から呼び出されることを前提) ---

    // 縦追従へ遷移 (横→縦, Yを固定値へ)
    // 外部スクリプトから呼び出されることを想定し、 public を維持
    public void vertical_axis()
    {
        if (nowstate == axis.horizontal)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);

            // 目標X座標はプレイヤーのX座標を使用
            Vector3 finalTargetPosition = new Vector3(
                target.position.x - offset.x, // プレイヤー追従位置
                first_position_y,             // Yは固定値
                transform.position.z
            );

            nowstate = axis.horizontal_to_vertical_transition;

            // 遷移時間 0.1秒
            currentCoroutine = StartCoroutine(MoveToPositionOverTime(finalTargetPosition, 0.2f, axis.vertical));

            leftLimit = first_leftLimit;
            rightLimit = first_rightLimit;
        }
    }

    // 横追従へ遷移 (縦→横, Yを追従位置へ)
    // 外部スクリプトから呼び出されることを想定し、 public を維持
    public void horizontal_axis(float newLeftLimit, float newRightLimit)
    {
        if (nowstate == axis.vertical)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);

            leftLimit = newLeftLimit;
            rightLimit = newRightLimit;

            // --- 目標X座標の計算 (境界線に近い方に移動) ---
            float currentX = transform.position.x;
            float targetRightX = rightLimit - limit;
            float targetLeftX = leftLimit + limit;

            float targetX;

            // 現在のX座標がどちらの目標X座標に近いかを判定
            if (Mathf.Abs(currentX - targetRightX) < Mathf.Abs(currentX - targetLeftX))
            {
                targetX = targetRightX;
            }
            else
            {
                targetX = targetLeftX;
            }

            // 目標位置: 計算されたX (境界目標), プレイヤーY, Z維持
            Vector3 finalTargetPosition = new Vector3(
                targetX,
                target.position.y - offset.y,
                transform.position.z
            );

            nowstate = axis.vertical_to_horizontal_transition;

            // 遷移時間 0.1秒
            currentCoroutine = StartCoroutine(MoveToPositionOverTime(finalTargetPosition, 0.2f, axis.horizontal));
        }
    }

    // --- Coroutine ---

    // 0.1秒かけてスムーズに目標位置へ移動させる汎用コルーチン
    private IEnumerator MoveToPositionOverTime(Vector3 finalTargetPosition, float duration, axis nextState)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            transform.position = Vector3.Lerp(startPosition, finalTargetPosition, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = finalTargetPosition;

        // コルーチン完了後、次の状態へ移行し、参照をクリア
        nowstate = nextState;
        currentCoroutine = null;
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using System.Xml;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Animations;

//public class new_following_camera : MonoBehaviour
//{
//    public Transform target;       // 追従する対象（プレイヤー）
//    public float smoothSpeed = 5f; // カメラの追従スピード
//    public Vector3 offset;         // プレイヤーからのオフセット
//    public float leftLimit = -10f; // カメラが行ける左端の座標
//    public float rightLimit = 10f; // カメラが行ける右端の座標
//    public float first_leftLimit = 0f;
//    public float first_rightLimit = 0f;
//    private axis nowstate;
//    public float first_position_y;
//    [SerializeField] private float limit = 10f;

//    [SerializeField] bool vertical = true;
//    [SerializeField] float wide = 0f;
//    [SerializeField] float heigth = 0f;

//    public enum axis
//    {
//        vertical,
//        horizontal,
//        horizontal_to_vertical
//    }

//    private bool followEnabled = true; // ← カメラ追従のオン・オフ切り替え用

//    private void Start()
//    {
//        nowstate = axis.horizontal;
//        first_position_y = transform.position.y;
//        first_leftLimit = leftLimit;
//        first_rightLimit = rightLimit;
//    }

//    void Update()
//    {
//        switch (nowstate)
//        {
//            case axis.vertical:
//                transform.position = new Vector3(target.transform.position.x - offset.x, first_position_y, transform.position.z);

//                if (transform.position.x >= rightLimit)
//                {
//                    transform.position = new Vector3(rightLimit, transform.position.y, transform.position.z);
//                }
//                if (transform.position.x <= leftLimit)
//                {
//                    transform.position = new Vector3(leftLimit, transform.position.y, transform.position.z);
//                }
//                break;
//            case axis.horizontal:
//                transform.position = new Vector3(target.transform.position.x - offset.x, target.transform.position.y - offset.y, transform.position.z);

//                if (rightLimit - transform.position.x <= limit)
//                {
//                    transform.position = new Vector3(rightLimit - limit, target.transform.position.y - offset.y, transform.position.z);
//                }
//                if (transform.position.x - leftLimit <= limit)
//                {
//                    transform.position = new Vector3(leftLimit + limit, target.transform.position.y - offset.y, transform.position.z);
//                }
//                break;
//            case axis.horizontal_to_vertical:
//                Vector3 finalTargetPosition = new Vector3(target.transform.position.x - offset.x,first_position_y,transform.position.z) + offset;

//                // スクリプトのインスタンスを取得し、コルーチンを開始
//                StartTimedFollow(finalTargetPosition, 1.0f);
//                nowstate = axis.vertical;
//                leftLimit = first_leftLimit;
//                rightLimit = first_rightLimit;
//                break;

//        }
//        //if (vertical)
//        //{
//        //    transform.position = new Vector3(target.transform.position.x - offset.x, transform.position.y, transform.position.z);

//        //    if (transform.position.x >= rightLimit)
//        //    {
//        //        transform.position = new Vector3(rightLimit, transform.position.y, transform.position.z);
//        //    }
//        //    if (transform.position.x <= leftLimit)
//        //    {
//        //        transform.position = new Vector3(leftLimit, transform.position.y, transform.position.z);
//        //    }
//        //}
//        //else
//        //{
//        //    transform.position = new Vector3(target.transform.position.x - offset.x, target.transform.position.y - offset.y, transform.position.z);

//        //    if (rightLimit - transform.position.x <= limit)
//        //    {
//        //        transform.position = new Vector3(rightLimit - limit, target.transform.position.y - offset.y, transform.position.z);
//        //    }
//        //    if (transform.position.x - leftLimit <= limit)
//        //    {
//        //        transform.position = new Vector3(leftLimit + limit, target.transform.position.y - offset.y, transform.position.z);
//        //    }
//        //}
//        }

//    private IEnumerator MoveToPositionOverTime(Vector3 targetPosition, float duration)
//    {
//        float timeElapsed = 0f;

//        // 開始位置を現在のカメラ位置のX, Z軸とし、Y軸は目標Y軸に即時合わせる
//        Vector3 startPosition = new Vector3(
//            transform.position.x,
//            targetPosition.y, // Y軸は即座に目標値に固定
//            transform.position.z
//        );

//        // カメラのY座標を即座に目標Y座標に設定
//        transform.position = startPosition;

//        // duration（この場合は1秒）が経過するまでループ
//        while (timeElapsed < duration)
//        {
//            float t = timeElapsed / duration;

//            // X軸のみLerpを実行し、Y軸は既に目標値なのでそのまま
//            float newX = Mathf.Lerp(startPosition.x, targetPosition.x, t);

//            // Z軸もLerpを実行
//            float newZ = Mathf.Lerp(startPosition.z, targetPosition.z, t);

//            transform.position = new Vector3(newX, startPosition.y, newZ);

//            timeElapsed += Time.deltaTime;
//            yield return null;
//        }

//        // 終了位置に正確に固定
//        transform.position = targetPosition;
//    }
//    public void SetFollowEnabled(bool enabled)
//    {
//        followEnabled = enabled;
//    }


//    //カメラ移動を横方向へ
//    public void horizontal_axis(float NewLeftLimit,float NewRightLimit)
//    {
//        leftLimit = NewLeftLimit;
//        rightLimit = NewRightLimit;
//        vertical = false;
//    }


//    public void vertical_axis()
//    {
//        //カメラ移動範囲の再設定
//        nowstate = axis.horizontal_to_vertical;
//    }


//}
