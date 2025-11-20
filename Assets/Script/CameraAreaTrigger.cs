using UnityEngine;

public class CameraAreaTrigger : MonoBehaviour
{
    public new_following_camera cameraScript;  
    public float areaLeftLimit = -5f;
    public float areaRightLimit = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CameraArea"))
        {
            // この範囲だけ縦に動く（vertical = false）
            cameraScript.horizontal_axis(areaLeftLimit, areaRightLimit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CameraArea"))
        {
            // 元の横移動だけに戻す（vertical = true）
            cameraScript.vertical_axis();
        }
    }
}
