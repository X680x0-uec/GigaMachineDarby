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
    [SerializeField] bool NewModel = true;
    private bool detect = false;
    /*target == true�Ȃ�΍��ɂ͂˂Ȃ������ʍs�œ���
    target == true ���� NewModel == false�Ȃ猟�m�͈͓��Ƀv���C���[������Ƃ������ǂ�������
    target == true ���� NewModel == true�Ȃ��x���m�͈͓��Ńv���C���[�����m������ǂ�����������
    ��3�`�Ԃ�p�ӂ��Ă��܂�*/

    private Rigidbody2D rb;
    private Enemy enemy; //���G��X�e�[�^�X���Ǘ�����X�N���v�g

    private void Start()
    {
        // Rigidbody2D �� Animator �� ���擾
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        if (target && !NewModel)
        {
            MoveSpeed *= 2;
        }
    }

    //�I�u�W�F�N�g�ƐڐG������target�̏�Ԃɂ���ċ�����ύX
    void OnTriggerEnter2D(Collider2D other)
    {
        if (target && !NewModel)
        {
            TargetSpring();
        }
        else if (target && NewModel)
        {
            NewModelSpring();
        }
        else if(!target)
        {
            UnTargetSpring();
        }
    }

    //�E�Ɉ���ʍs�œ����ꍇ
    private void UnTargetSpring()
    {
        if (!enemy.IsDetected && enemy.DetectedPlayer == null){return;}
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
            enemy.detectionRadius = 5f;
        }
        //�v���C���[�����G�͈͂̎��ǂ�������
        else if (enemy.IsDetected && enemy.DetectedPlayer != null)
        {
            enemy.detectionRadius = 15f;
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

    private void NewModelSpring()
    {
        if (!enemy.IsDetected && enemy.DetectedPlayer == null && !detect)
        {
            rb.linearVelocity = new Vector2(0, MiniJumpForce);
        }
        else if (enemy.IsDetected && enemy.DetectedPlayer != null && !detect)
        {
            detect = true;
            enemy.detectionRadius = Mathf.Infinity;
            MoveTowardsPlayer(JumpForce);
            JumpCount++;
        }
        else if (detect)
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
        // 現在のスケールを一旦取得する
        Vector3 currentScale = transform.localScale;

        // 水平速度が0.01より大きい場合（右向き）
        if (horizontalVelocity > 0.01f)
        {
            currentScale.x = Mathf.Abs(currentScale.x);
        }
        // 水平速度が-0.01より小さい場合（左向き）
        else if (horizontalVelocity < -0.01f)
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
        }
        
        transform.localScale = currentScale;
    
    }
}
