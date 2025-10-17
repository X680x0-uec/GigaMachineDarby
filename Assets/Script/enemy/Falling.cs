using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Falling : MonoBehaviour
{
    // �Q�Ƃ���R���|�[�l���g
    private Rigidbody2D rb;
    private Enemy enemy; //���G��X�e�[�^�X���Ǘ�����X�N���v�g
    private enemy_contact EnemyContact; //�ڐG�ɂ��_���[�W�̔���ƒl���Ǘ�����X�N���v�g

    bool falled = false; 
    bool falling = false;
    [SerializeField] private float falling_speed = 6.0f;
    [SerializeField] private float rising_speed = 3.0f;
    float start_x;
    float start_y;
    //private Vector2 currentVelocity; //���W�̕ύX�p
    private Vector2 target;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Rigidbody2D �� Animator �� ���擾
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        rb.bodyType = RigidbodyType2D.Static; //�ق��I�u�W�F�N�g�ƏՓ˂����ۂ�falling�������o����Ȃ��悤�ɂ���
        //rb.constraints = RigidbodyConstraints2D.FreezePosition;
        float start_x = transform.position.x;
        float start_y = transform.position.y;
        target.x = start_x;
        target.y = start_y;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (!falled)
        {
            if (enemy.IsDetected && enemy.DetectedPlayer != null)
            {
                float falling_x = transform.position.x;
                float falling_y = transform.position.y;
                float player_x = enemy.DetectedPlayer.transform.position.x;
                float player_y = enemy.DetectedPlayer.transform.position.y;
                if (System.Math.Abs(falling_x - player_x) < 1.5 && falling_y > player_y)
                {
                    Debug.Log("���m");
                    falling = true;
                }
                if (falling)
                {
                    Debug.Log("ok");
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    //currentVelocity.y = falling_speed;
                    //transform.position -= (Vector3)currentVelocity * Time.deltaTime;
                    //rb.linearVelocity = new Vector2(0, -falling_speed);
                    rb.AddForce(Vector2.down * falling_speed);
                }
            }
        }
        else
        {
            //transform.position = Vector2.MoveTowards(
            //    transform.position,
            //    target,
            //    rising_speed * Time.deltaTime);
            rb.linearVelocity = Vector2.up * rising_speed;
            if (transform.position.y >= target.y)
            {
                falled = false;
                rb.bodyType = RigidbodyType2D.Static;
                transform.position = transform.position;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("floor") || other.gameObject.CompareTag("Player"))
        {
            falling = false;
            falled = true;
        }
    }
}
