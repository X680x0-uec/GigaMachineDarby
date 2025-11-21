using UnityEngine;
using UnityEngine.UI; // UIを使うために必要
using UnityEngine.SceneManagement;

public class ResultDisplay : MonoBehaviour
{
    public Text timeText;   // 時間を表示するテキスト
    public Text enemyText;  // 敵の数を表示するテキスト

    void Start()
    {
        if (GameManager.instance != null)
        {
            // 時間を "00.00" 形式で表示
            float t = GameManager.instance.totalTime;
            timeText.text = "Time: " + t.ToString("F2") + "s";

            // 敵の撃破数を表示
            enemyText.text = "Defeated: " + GameManager.instance.defeatedEnemies + " Enemies";
        }
    }

    // ボタンに割り当てる用：リトライ
    public void OnRetryButton()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RetryGame();
        }
    }
    
    // ボタンに割り当てる用：タイトルへ（必要なら）
    public void OnTitleButton()
    {
        SceneManager.LoadScene("TitleScene");
    }
}