using UnityEngine;
using UnityEngine.UI;
using System;

public class Spring : MonoBehaviour
{

    [Header("�ړ��ݒ�")]
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float JumpForce = 7f;
    [SerializeField] private float MiniJumpForce = 4f;
    [SerializeField] private int JumpCount = 1;
    [SerializeField] private int MaxJump = 3;
    [SerializeField] bool target = true;

    private Rigidbody2D rb;
    private Enemy enemy; //���G��X�e�[�^�X���Ǘ�����X�N���v�g

    private void Start()
    {
        // Rigidbody2D �� Animator �� ���擾
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
    }

    //�I�u�W�F�N�g�ƐڐG������target�̏�Ԃɂ���ċ�����ύX
    void OnTriggerEnter2D(Collider2D other)
    {
        if (target)
        {
            TargetSpring();
        }
        else
        {
            UnTargetSpring();
        }
    }

    //�E�Ɉ���ʍs�œ����ꍇ
    private void UnTargetSpring()
    {
        if (JumpCount < MaxJump)
        {
            rb.linearVelocity = new Vector2(-MoveSpeed, JumpForce);
            JumpCount++;
        }
        else if (JumpCount == MaxJump)
        {
            rb.linearVelocity = new Vector2(-MoveSpeed, 2 * JumpForce);
            JumpCount = 1;
        }
    }

    //�v���C���[��ǂ�������ꍇ
    private void TargetSpring()
    {
        //�v���C���[�����G�͈͊O�̎����̏�ł͂˂�
        if (!enemy.IsDetected && enemy.DetectedPlayer == null)
        {
            rb.linearVelocity = new Vector2(0, MiniJumpForce);
        }
        //�v���C���[�����G�͈͂̎��ǂ�������
        else if (enemy.IsDetected && enemy.DetectedPlayer != null)
        {
            if (JumpCount < MaxJump)
            {
                MoveTowardsPlayer(JumpForce);
                JumpCount++;
            }
            else
            {
                MoveTowardsPlayer(2 * JumpForce);
                JumpCount = 1;
            }
        }
    }

    private void MoveTowardsPlayer(float JumpForce)
    {
        // ���΍��W�����K��
        float direction = (enemy.DetectedPlayer.transform.position.x - transform.position.x);
        direction = direction < 0 ? -1 : 1;

        rb.linearVelocity = new Vector2(direction * MoveSpeed, JumpForce);

        // �ړ������ɉ����ăX�v���C�g�̌�����ς���
        FlipSprite(rb.linearVelocity.x);
    }

    private void FlipSprite(float horizontalVelocity)
    {
        // ���x���E�����i0���傫���j�Ȃ�X�v���C�g�͂��̂܂�
        if (horizontalVelocity > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // ���x���������i0��菬�����j�Ȃ�X�v���C�g�𔽓]������
        else if (horizontalVelocity < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        // �قڒ�~���Ă���ꍇ�͉������Ȃ�
    }
}
