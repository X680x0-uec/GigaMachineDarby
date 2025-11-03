using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class Hammer : MonoBehaviour
{
    // �Q�Ƃ���R���|�[�l���g
    private Rigidbody2D rb;
    private Enemy enemy; //���G��X�e�[�^�X���Ǘ�����X�N���v�g
    private enemy_contact EnemyContact; //�ڐG�ɂ��_���[�W�̔���ƒl���Ǘ�����X�N���v�g


    [Header("�V���b�g�ݒ�")]
    public GameObject boxPrefab;
    public Transform throwPoint;
    public float throwForce = 10f;
    [SerializeField] private float ThrowInterval = 2f;
    private float timer = 0f;

    float direction = -1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeDirection();
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer > ThrowInterval)
        {
            ThrowBox();
            timer = 0f;
        }
    }

    void ThrowBox()
    {
        GameObject box = Instantiate(boxPrefab, throwPoint.position, Quaternion.identity);
        BoxBehavior boxBehavior = box.GetComponent<BoxBehavior>();
        Rigidbody2D boxRb = box.GetComponent<Rigidbody2D>();

        if (boxPrefab != null && throwPoint != null)
        {
            boxRb.AddForce(new Vector2(direction, 0.5f).normalized * throwForce, ForceMode2D.Impulse);
        }
    }

    void ChangeDirection()
    {
        float hammer_x = transform.position.x;
        if (enemy.DetectedPlayer == null)
        {
            return;
        }
        float player_x = enemy.DetectedPlayer.transform.position.x;
        if (hammer_x > player_x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
            Vector3 scale = transform.localScale;
        scale.x = direction;
        transform.localScale = scale;
    }
}
