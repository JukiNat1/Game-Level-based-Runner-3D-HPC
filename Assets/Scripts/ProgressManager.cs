using UnityEngine;

/// <summary>
/// Singleton that manages game progress (unlocked levels and high scores).
/// DontDestroyOnLoad ensures it persists across all scenes.
/// Uses PlayerPrefs for data persistence.
/// </summary>
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    /// <summary>Current level index (0-based). Level 1 = index 0.</summary>
    public int currentLevelIndex = 0;

    private const string UNLOCK_KEY = "level_{0}_unlocked";
    private const string SCORE_KEY  = "level_{0}_highscore";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Level 1 is always unlocked
        if (!PlayerPrefs.HasKey(string.Format(UNLOCK_KEY, 0)))
        {
            PlayerPrefs.SetInt(string.Format(UNLOCK_KEY, 0), 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Returns whether level N is unlocked.
    /// </summary>
    public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex == 0) return true; // Level 1 is always unlocked
        return PlayerPrefs.GetInt(string.Format(UNLOCK_KEY, levelIndex), 0) == 1;
    }

    /// <summary>
    /// Returns the highest score for level N.
    /// </summary>
    public int GetHighScore(int levelIndex)
    {
        return PlayerPrefs.GetInt(string.Format(SCORE_KEY, levelIndex), 0);
    }

    /// <summary>
    /// Called after completing a level: saves the score and unlocks the next level.
    /// </summary>
    public void SaveLevelComplete(int levelIndex, int coins)
    {
        // Update high score if the new score is higher
        int current = GetHighScore(levelIndex);
        if (coins > current)
        {
            PlayerPrefs.SetInt(string.Format(SCORE_KEY, levelIndex), coins);
        }

        // Unlock the next level
        int next = levelIndex + 1;
        if (next < LevelData.TOTAL_LEVELS)
        {
            PlayerPrefs.SetInt(string.Format(UNLOCK_KEY, next), 1);
        }

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Resets all progress (debug/testing only).
    /// </summary>
    public void ResetAllProgress()
    {
        for (int i = 0; i < LevelData.TOTAL_LEVELS; i++)
        {
            PlayerPrefs.DeleteKey(string.Format(UNLOCK_KEY, i));
            PlayerPrefs.DeleteKey(string.Format(SCORE_KEY, i));
        }
        // Re-unlock Level 1
        PlayerPrefs.SetInt(string.Format(UNLOCK_KEY, 0), 1);
        PlayerPrefs.Save();
        Debug.Log("ProgressManager: Đã reset toàn bộ tiến trình.");
    }
}
