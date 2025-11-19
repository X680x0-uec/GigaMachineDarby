using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class new_following_camera : MonoBehaviour
{
    public Transform target;       // 追従する対象（プレイヤー）
    public float smoothSpeed = 5f; // カメラの追従スピード
    public Vector3 offset;         // プレイヤーからのオフセット
    public float leftLimit = -10f; // カメラが行ける左端の座標
    public float rightLimit = 10f; // カメラが行ける右端の座標
    public float first_leftLimit = 0f;
    public float first_rightLimit = 0f;

    [SerializeField] bool vertical = true;
    [SerializeField] float wide = 0f;
    [SerializeField] float heigth = 0f;

    private bool followEnabled = true; // ← カメラ追従のオン・オフ切り替え用

    private void Start()
    {
        first_leftLimit = leftLimit;
        first_rightLimit = rightLimit;
    }

    void Update()
    {
        if (vertical)
        {
            transform.position = new Vector3(target.transform.position.x - offset.x, transform.position.y, transform.position.z);

            if (transform.position.x >= rightLimit)
            {
                transform.position = new Vector3(rightLimit, transform.position.y, transform.position.z);
            }
            if (transform.position.x <= leftLimit)
            {
                transform.position = new Vector3(leftLimit, transform.position.y, transform.position.z);
            }
        }
        else
        {
            transform.position = new Vector3(target.transform.position.x - offset.x, target.transform.position.y - offset.y, transform.position.z);

            if (transform.position.x >= rightLimit)
            {
                transform.position = new Vector3(rightLimit, target.transform.position.y - offset.y, transform.position.z);
            }
            if (transform.position.x <= leftLimit)
            {
                transform.position = new Vector3(leftLimit, target.transform.position.y - offset.y, transform.position.z);
            }
        }
        }


    public void SetFollowEnabled(bool enabled)
    {
        followEnabled = enabled;
    }


    //カメラ移動を横方向へ
    public void horizontal_axis(float NewLeftLimit,float NewRightLimit)
    {
        leftLimit = NewLeftLimit;
        rightLimit = NewRightLimit;
        vertical = false;
    }
    

    public void vertical_axis()
    {
        //カメラ移動範囲の再設定
        leftLimit = first_leftLimit;
        rightLimit = first_rightLimit;
        vertical = true;
    }

    
}
