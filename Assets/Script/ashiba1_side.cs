using UnityEngine;

public class ashiba1_side : MonoBehaviour
{
    void Start()
    {
        Vector2 top_scale = transform.localScale;
        top_scale.x = top_scale.x + 0.04f;
        transform.localScale = top_scale;
    }
}
