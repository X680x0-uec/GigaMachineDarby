using UnityEngine;

public class Sample : MonoBehaviour
{
    [SerializeField] private new_following_camera camera;
    [SerializeField] private HorizontalSquare horizontal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject axis = collision.gameObject;
        Debug.Log(axis);
        if (collision.tag != "vertical" &&  collision.tag != "horizontal")
        {
            return;
        }
        else if (axis != null && collision.tag == "vertical")
        {
            camera.vertical_axis();
        }
        else if(axis != null && collision.tag == "horizontal")
        {
            horizontal = collision.GetComponent<HorizontalSquare>();
            float wide = horizontal.right_x - horizontal.left_x;
            float LeftLimit = horizontal.left_x;
            float RightLimit = horizontal.right_x;
            camera.horizontal_axis(LeftLimit, RightLimit);
        }
    }
}
