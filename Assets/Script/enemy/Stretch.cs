using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


public class Stretch : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy enemy;

    [Header("変更後のサイズ")]
    [SerializeField] float newScalse = 5f;
    [SerializeField] float nomalScale = 1f;

    bool normal = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();

        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!enemy.IsDetected && enemy.DetectedPlayer == null)
        {
            if (!normal)
            {
                transform.localScale = new Vector3(nomalScale, nomalScale, nomalScale);
            }
        }
        else if (enemy.IsDetected && enemy.DetectedPlayer != null)
        {
            transform.localScale = new Vector3(newScalse, newScalse, newScalse);
            normal = false;
        }
    }


}
