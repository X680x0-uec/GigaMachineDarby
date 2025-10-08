using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


public class Stretch : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy enemy;

    public Transform AttackTransform;

    [Header("変更後のサイズ")]
    [SerializeField] float newScalse = 5f;
    [SerializeField] float nomalScale = 0.25f;

    private Transform player;
    [SerializeField] private Collider2D AttackCollider;

    private float x;
    private float y;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        
        //コライダーの取得
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!enemy.IsDetected && enemy.DetectedPlayer == null)
        {
            AttackTransform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            AttackTransform.localScale = new Vector3(nomalScale, nomalScale, 1f);
        }
        else if (enemy.IsDetected && enemy.DetectedPlayer != null)
        {
            x = player.position.x + transform.position.x;
            y = player.position.y + transform.position.y;
            if (distanceToPlayer < 3f)
            {
                Vector3 direction = player.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                AttackTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                AttackTransform.position = new Vector3(x / 2f, y / 2f, 0f);
                AttackTransform.localScale = new Vector3(distanceToPlayer, 0.25f, 1f);
            }
            else
            {
                AttackTransform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                AttackTransform.localScale = new Vector3(0.25f, 0.25f, 1f);
            }
        }
    }


}
