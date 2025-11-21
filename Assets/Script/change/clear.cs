using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class clear : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] private float player_x = 0f;
    [SerializeField] private float player_y = 0f;
    [SerializeField] private float door_x = 0f;
    [SerializeField] private float door_y = 0f;
    public string Nextscene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform taransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        player_x = player.position.x;
        player_y = player.position.y;
        door_x = transform.position.x;
        door_y = transform.position.y;

        if (Math.Abs(door_x - player_x) < 0.1f && Math.Abs(door_y - player_y) < 100f)
        {
            SceneManager.LoadScene("clear", LoadSceneMode.Single);
        }
    }
}