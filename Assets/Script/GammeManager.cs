using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要

public class GameManager : MonoBehaviour
{
    // どこからでもアクセスできるようにする（シングルトン）
    public static GameManager instance;

    [Header("ゲームデータ")]
    public float totalTime = 0f;   // 経過時間
    public int defeatedEnemies = 0; // 倒した敵の数
    public bool isGameActive = true; // 計測中かどうか

    void Awake()
    {
        // 既にGameManagerが存在していたら、自分を削除（重複防止）
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移しても破壊しない設定
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // ゲームプレイ中のみ時間を計測
        if (isGameActive)
        {
            totalTime += Time.deltaTime;
        }
    }

    // 敵を倒した時に呼ぶ関数
    public void AddEnemyCount()
    {
        defeatedEnemies++;
        Debug.Log("現在の撃破数: " + defeatedEnemies);
    }

    // ゲームオーバー時に呼ぶ
    public void GameOver()
    {
        isGameActive = false; // 時間計測ストップ
        SceneManager.LoadScene("Bad"); // ゲームオーバーシーンへ
    }

    // ゲームクリア時に呼ぶ
    public void GameClear()
    {
        isGameActive = false; // 時間計測ストップ
        SceneManager.LoadScene("GameClearScene"); // クリアシーンへ
    }

    // 最初からやり直す時に呼ぶ（リセット）
    public void RetryGame()
    {
        totalTime = 0f;
        defeatedEnemies = 0;
        isGameActive = true;
        SceneManager.LoadScene("Stage1"); // 最初のステージへ（名前は合わせてください）
    }
}