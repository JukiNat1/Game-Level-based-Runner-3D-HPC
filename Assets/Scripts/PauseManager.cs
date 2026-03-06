using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Pause functionality in the Level scene.
/// - PC: press ESC to toggle
/// - Mobile: tap the hamburger button (three horizontal lines)
/// Reuses the Game Over panel UI, switching the title to "Paused"
/// when paused and "Game Over" when the player loses.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public static bool isPaused = false;

    [Header("Pause Panel (tận dụng lại Game Over Panel)")]
    public GameObject pausePanel;           // Assign same gameOverPanel from Events
    public Text titleText;                  // "Game Over" text, changed to "Paused" when paused

    [Header("Các nút trong Pause Menu")]
    public Button btnResume;                // Resume
    public Button btnRestart;              // Restart
    public Button btnSettings;             // Settings
    public Button btnQuitToMenu;           // Quit to Main Menu
    public Button btnLeaderboard;          // Leaderboard

    [Header("Nút Hamburger (HUD - hiện khi đang chơi)")]
    public Button hamburgerButton;          // Hamburger button shown on HUD while playing

    [Header("Settings Panel (tuỳ chọn)")]
    public GameObject settingsPanel;        // Settings panel (if any)

    [Header("Nút Game Over riêng (ẩn khi pause)")]
    public Button btnGameOverQuit;          // Original Quit button (Application.Quit)
    public Button btnGameOverLeaderboard;   // Original Leaderboard button

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    void Update()
    {
        // Only allow pause when game is running and not already over
        if (!PlayerManager.isGameStarted || PlayerManager.gameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    /// <summary>
    /// Called from the hamburger button or the ESC key.
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// Pauses the game – shows pause panel with "Paused" title.
    /// </summary>
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;

        if (titleText != null)
            titleText.text = "Tạm dừng";

        // Hide original Game Over buttons, show Pause buttons
        SetPauseButtonsVisible(true);

        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Hide hamburger button while inside the pause menu
        if (hamburgerButton != null)
            hamburgerButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Resumes gameplay – hides the pause panel.
    /// </summary>
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Show hamburger button again
        if (hamburgerButton != null)
            hamburgerButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called from Events on Game Over – switches the title to "Game Over".
    /// </summary>
    public void ShowAsGameOver()
    {
        isPaused = false;

        if (titleText != null)
            titleText.text = "Game Over";

        // Hide Resume button on game over
        SetPauseButtonsVisible(false);

        if (hamburgerButton != null)
            hamburgerButton.gameObject.SetActive(false);
    }

    // Show/hide buttons specific to pause vs game-over state
    private void SetPauseButtonsVisible(bool pauseMode)
    {
        if (btnResume != null)
            btnResume.gameObject.SetActive(pauseMode);

        // Quit-to-menu and Leaderboard are shared between both states
        // Original Quit button (Application.Quit) – hidden when paused, replaced by GoToMenu
        if (btnGameOverQuit != null)
            btnGameOverQuit.gameObject.SetActive(!pauseMode);

        // Settings only visible when paused
        if (btnSettings != null)
            btnSettings.gameObject.SetActive(pauseMode);
    }

    // ───── Button callbacks ─────

    /// <summary>"Resume" button</summary>
    public void OnResumeButton()
    {
        Resume();
    }

    /// <summary>"Restart" button – reloads the Level scene for the same level</summary>
    public void OnRestartButton()
    {
        isPaused = false;
        Time.timeScale = 1;
        Highscore.isScoreAlreadyAdded = false;
        SceneManager.LoadScene("Level");
    }

    /// <summary>"Quit" button – returns to the main menu (not Application.Quit)</summary>
    public void OnQuitToMenuButton()
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    /// <summary>"Leaderboard" button</summary>
    public void OnLeaderboardButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("HighScores", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    /// <summary>"Settings" button – toggles the settings panel</summary>
    public void OnSettingsButton()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
