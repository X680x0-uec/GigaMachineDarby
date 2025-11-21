using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_to_tutorial : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] private float player_x = 0f;
    [SerializeField] private float player_y = 0f;
    [SerializeField] private float door_x = 0f;
    [SerializeField] private float door_y = 0f;

    void Start()
    {
        Transform taransform = GetComponent<Transform>();
    }

    void Update()
    {
        // プレイヤーやドアが削除されている場合のエラー防止
        if (player == null) return;

        player_x = player.position.x;
        player_y = player.position.y;
        door_x = transform.position.x;
        door_y = transform.position.y;

        // 距離判定（ドアに近づいたら）
        if (Math.Abs(door_x - player_x) < 0.1f && Math.Abs(door_y - player_y) < 3f)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.totalTime = 0f;
                GameManager.instance.defeatedEnemies = 0;
                GameManager.instance.isGameActive = true;
            }

            SceneManager.LoadScene("stage1", LoadSceneMode.Single);
        }
    }
}