using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Level Select screen.
/// Attach to any GameObject in the LevelSelect scene.
/// Requires 6 LevelButton entries assigned in the Inspector.
/// </summary>
public class LevelSelectManager : MonoBehaviour
{
    [Header("Gán 6 nút màn chơi (Màn 1 → Màn 6)")]
    public Button[] levelButtons;       // 6 buttons
    public Text[]   levelNameTexts;     // Level name text on each button
    public Text[]   highScoreTexts;     // High score text (can be null)
    public Text[]   lockTexts;          // Lock icon text shown when level is locked (optional)

    [Header("Nút Quay Lại")]
    public Button backButton;

    void Start()
    {
        // Ensure ProgressManager exists (in case this scene is entered directly from the Editor)
        if (ProgressManager.Instance == null)
        {
            GameObject pm = new GameObject("ProgressManager");
            pm.AddComponent<ProgressManager>();
        }

        RefreshLevelButtons();
    }

    /// <summary>
    /// Refreshes the state of all level buttons based on current progress.
    /// </summary>
    private void RefreshLevelButtons()
    {
        for (int i = 0; i < LevelData.TOTAL_LEVELS; i++)
        {
            if (i >= levelButtons.Length || levelButtons[i] == null) continue;

            bool unlocked = ProgressManager.Instance.IsLevelUnlocked(i);
            levelButtons[i].interactable = unlocked;

            // Button color: bright when unlocked, grey when locked
            ColorBlock colors = levelButtons[i].colors;
            Color normal = unlocked
                ? new Color(0.2f, 0.7f, 0.3f, 1f)   // green when unlocked
                : new Color(0.4f, 0.4f, 0.4f, 1f);   // grey when locked
            colors.normalColor   = normal;
            colors.pressedColor  = unlocked ? new Color(0.1f, 0.5f, 0.2f, 1f) : colors.pressedColor;
            levelButtons[i].colors = colors;

            // Level name
            if (levelNameTexts != null && i < levelNameTexts.Length && levelNameTexts[i] != null)
            {
                levelNameTexts[i].text = unlocked
                    ? LevelData.LevelNames[i]
                    : "🔒 Màn " + (i + 1);
            }

            // High score
            if (highScoreTexts != null && i < highScoreTexts.Length && highScoreTexts[i] != null)
            {
                int hs = ProgressManager.Instance.GetHighScore(i);
                highScoreTexts[i].text = unlocked && hs > 0
                    ? "Best: " + hs + " coins"
                    : "";
            }

            // Lock icon text
            if (lockTexts != null && i < lockTexts.Length && lockTexts[i] != null)
            {
                lockTexts[i].gameObject.SetActive(!unlocked);
            }
        }
    }

    /// <summary>
    /// Called from Button onClick. levelIndex is 0-based.
    /// </summary>
    public void SelectLevel(int levelIndex)
    {
        if (!ProgressManager.Instance.IsLevelUnlocked(levelIndex))
        {
            Debug.LogWarning("LevelSelectManager: Màn " + (levelIndex + 1) + " chưa được mở khóa.");
            return;
        }

        ProgressManager.Instance.currentLevelIndex = levelIndex;
        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// Back button – returns to the main menu.
    /// </summary>
    public void GoBack()
    {
        SceneManager.LoadScene("Menu");
    }
}
