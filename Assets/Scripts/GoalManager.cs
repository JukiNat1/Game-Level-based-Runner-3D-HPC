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
    public Text levelCompleteText;    // Displays "Level X Complete!"
    public Text coinsEarnedText;      // Displays coins earned

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
    /// Called when the player reaches the finish gate. Shows victory panel and stops the game.
    /// </summary>
    public void TriggerVictory()
    {
        if (victoryTriggered) return;
        victoryTriggered = true;

        // Stop player movement
        PlayerManager.gameOver = true;

        // Ensure ProgressManager exists (create if missing)
        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("GoalManager: ProgressManager chưa có trong scene, tự tạo...");
            new GameObject("ProgressManager").AddComponent<ProgressManager>();
        }

        // Save progress: unlock next level and update high score
        int lvl = ProgressManager.Instance.currentLevelIndex;
        ProgressManager.Instance.SaveLevelComplete(lvl, PlayerManager.numberOfCoins);
        Debug.Log("GoalManager: Hoàn thành màn " + (lvl + 1) + 
                  " | Coins: " + PlayerManager.numberOfCoins +
                  " | Đã unlock màn " + (lvl + 2));

        // Pause time (freezes all physics and animations)
        StartCoroutine(ShowVictoryAfterDelay(0.5f));
    }

    private IEnumerator ShowVictoryAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 0;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // Update level info text
        if (ProgressManager.Instance != null)
        {
            int lvl = ProgressManager.Instance.currentLevelIndex;
            if (levelCompleteText != null)
                levelCompleteText.text = "Màn " + (lvl + 1) + " Hoàn Thành! 🎉";
            if (coinsEarnedText != null)
                coinsEarnedText.text = "Coins: " + PlayerManager.numberOfCoins;
        }

        if (victoryMessageText != null)
            victoryMessageText.text = victoryMessage;
    }

    /// <summary>
    /// "Replay" button – reloads the current Level scene.
    /// </summary>
    public void OnReplayButton()
    {
        Time.timeScale = 1;
        victoryTriggered = false;
        Highscore.isScoreAlreadyAdded = false;
        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// "Next Level" button – loads the next level if available.
    /// </summary>
    public void OnNextLevelButton()
    {
        Time.timeScale = 1;
        victoryTriggered = false;
        Highscore.isScoreAlreadyAdded = false;

        if (ProgressManager.Instance != null)
        {
            int next = ProgressManager.Instance.currentLevelIndex + 1;
            if (next < LevelData.TOTAL_LEVELS)
            {
                ProgressManager.Instance.currentLevelIndex = next;
                SceneManager.LoadScene("Level");
                return;
            }
        }
        // All levels done → return to Level Select
        SceneManager.LoadScene("LevelSelect");
    }

    /// <summary>
    /// "Quit" button – exits the application.
    /// </summary>
    public void OnQuitButton()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
