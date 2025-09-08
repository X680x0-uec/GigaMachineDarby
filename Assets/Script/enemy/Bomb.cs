using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    [SerializeField] private float moveSpeed = 8f; // �G�̈ړ����x

    // �Q�Ƃ���R���|�[�l���g
    private Rigidbody2D rb;
    private Enemy enemy; //���G��X�e�[�^�X���Ǘ�����X�N���v�g
    private enemy_contact EnemyContact; //�ڐG�ɂ��_���[�W�̔���ƒl���Ǘ�����X�N���v�g

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        EnemyContact = GetComponent<enemy_contact>();
        EnemyContact.damage = 50; //�z�u���邽�тɃC���X�y�N�^�[����ύX���Ȃ��čςނ��߂̏���
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.TakeDamage(100); //�v���C���[�ɐڐG����Ƃ��̃I�u�W�F�N�g�����S
        }
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y); //�������Ɉړ��B
                                                                          //���̃X�N���v�g���ƒn�ʂɐڐG���Ă��Ȃ��Ǝg���Ȃ��̂�
                                                                          //�󒆂ł��g����悤�Ɍ���C���\��
    }
}
