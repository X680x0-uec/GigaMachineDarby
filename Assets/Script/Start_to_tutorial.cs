using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_to_tutorial : MonoBehaviour
{
    [SerializeField] Transform player;

    private float player_x = 0f;
    private float player_y = 0f;
    private float door_x = 0f;
    private float door_y = 0f;
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

        if (door_x - player_x < 0.5f)
        {
            SceneManager.LoadScene("teki_exp", LoadSceneMode.Single);
        }
    }
}
