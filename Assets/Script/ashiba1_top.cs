using UnityEngine;
using UnityEngine.Animations;

public class ashiba1_top : MonoBehaviour
{
    void Start()
    {
        Vector2 top_scale = transform.localScale;
        top_scale.y = top_scale.y + 0.0004f;
        transform.localScale = top_scale;
    }
}
