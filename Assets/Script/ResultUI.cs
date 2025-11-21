using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // ★追加: コルーチンを使うために必要

public class GameResultManager : MonoBehaviour
{
    [Header("TextMeshProのUIをセットしてください")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI enemyText;

    [Header("演出設定")]
    [SerializeField] private float typeSpeed = 0.05f; // 1文字表示する間隔（秒）
    private AudioSource audioSource;

    [Header("遷移先のステージ名")]
    [SerializeField] private string stageSceneName = "stage1"; 
    [SerializeField] private string titleSceneName = "Start";

    void Start()
    {

        // 表示する文字列を準備
        string finalTimeStr = "";
        string finalEnemyStr = "";

        if (GameManager.instance != null)
        {
            float t = GameManager.instance.totalTime;
            finalTimeStr = "タイム: " + t.ToString("F2") + "s";
            finalEnemyStr = "倒した数: " + GameManager.instance.defeatedEnemies + "体";
        }
        else
        {
            // テスト用
            finalTimeStr = "タイム: --.--s";
            finalEnemyStr = "倒した数: --体";
        }

        // アニメーション開始！
        StartCoroutine(PlayTypewriterEffect(finalTimeStr, finalEnemyStr));
    }

    // タイプライター演出を行うコルーチン
    IEnumerator PlayTypewriterEffect(string tStr, string eStr)
    {
        if (timeText != null) timeText.text = "";
        if (enemyText != null) enemyText.text = "";

        yield return new WaitForSeconds(0.3f);

        //時間の表示
        if (timeText != null)
        {
            foreach (char c in tStr)
            {
                timeText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        // 少し間を空ける
        yield return new WaitForSeconds(0.3f);

        // --- 敵数の表示アニメーション ---
        if (enemyText != null)
        {
            foreach (char c in eStr)
            {
                enemyText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
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