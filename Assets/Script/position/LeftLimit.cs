using UnityEngine;

public class LeftLimit : MonoBehaviour
{
    [SerializeField] private Transform squre;
    public float position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = squre.transform.position.x - squre.transform.lossyScale.x / 2;
        transform.position = new Vector2 (position, squre.transform.position.y);
        transform.localScale = new Vector2(0.00001f, 1f);
    }
}
