using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro; // ★ここが重要：TextMeshProを使うための宣言

public class GameResultManager : MonoBehaviour
{
    [Header("TextMeshProのUIをセットしてください")]
    [SerializeField] private TextMeshProUGUI timeText;   // ★型を変更しました
    [SerializeField] private TextMeshProUGUI enemyText;  // ★型を変更しました

    [Header("遷移先のステージ名")]
    [SerializeField] private string stageSceneName = "stage1"; 
    [SerializeField] private string titleSceneName = "Start";

    void Start()
    {
        // --- 結果の表示処理 ---
        if (GameManager.instance != null)
        {
            float t = GameManager.instance.totalTime;
            
            // 時間表示
            if (timeText != null)
            {
                timeText.text = "Time: " + t.ToString("F2") + "s";
            }

            // 敵数表示
            if (enemyText != null)
            {
                enemyText.text = "Defeated: " + GameManager.instance.defeatedEnemies;
            }
        }
        else
        {
            // テスト用表示
            if (timeText != null) timeText.text = "Time: --.--s";
            if (enemyText != null) enemyText.text = "Defeated: --";
        }
    }

    // ■「リスタート」ボタン用
    public void OnRetryButton()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.totalTime = 0f;
            GameManager.instance.defeatedEnemies = 0;
            GameManager.instance.isGameActive = true;
        }
        SceneManager.LoadScene(stageSceneName);
    }

    // ■「終了」ボタン用
    public void OnQuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}