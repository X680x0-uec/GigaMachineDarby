using UnityEngine;

public class HorizontalSquare : MonoBehaviour
{
    [SerializeField] private RightLimit right;
    [SerializeField] private LeftLimit left;
    public float right_x;
    public float left_x;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        right_x  = right.position;
        left_x = left.position;
    }

    // Update is called once per frame
    void Update()
    {
        right_x = right.position;
        left_x = left.position;
    }
}
