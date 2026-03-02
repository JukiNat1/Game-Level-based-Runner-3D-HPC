using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance { get; private set; }

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public Text victoryMessageText;

    [Header("Settings")]
    public string victoryMessage = "Chúc mừng bạn đã đến cổng trường HPC";

    private bool victoryTriggered = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Hide panel on start
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    void Start()
    {
        if (victoryMessageText != null)
            victoryMessageText.text = victoryMessage;
    }

    /// <summary>
    /// Gọi khi Player chạm vào cổng đích. Hiển thị panel chúc mừng và dừng game.
    /// </summary>
    public void TriggerVictory()
    {
        if (victoryTriggered) return;
        victoryTriggered = true;

        // Dừng player di chuyển
        PlayerManager.gameOver = true;

        // Dừng thời gian (dừng toàn bộ physics & animation)
        StartCoroutine(ShowVictoryAfterDelay(0.5f));
    }

    private IEnumerator ShowVictoryAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 0;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    /// <summary>
    /// Nút "Chơi lại" - load lại scene Level
    /// </summary>
    public void OnReplayButton()
    {
        Time.timeScale = 1;
        victoryTriggered = false;
        Highscore.isScoreAlreadyAdded = false;
        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// Nút "Thoát" - đóng game
    /// </summary>
    public void OnQuitButton()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
