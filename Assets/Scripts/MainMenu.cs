using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Nút Chơi Mới (chỉ hiện sau khi hoàn thành Màn 1)")]
    public GameObject btnNewGame; // Assign the "New Game" Button from the Inspector

    void Start()
    {
        // Show/hide New Game button based on player progress
        if (btnNewGame != null)
        {
            // IsLevelUnlocked(1) == true means Level 2 is unlocked, i.e. Level 1 is complete
            bool level1Done = ProgressManager.Instance != null
                              && ProgressManager.Instance.IsLevelUnlocked(1);
            btnNewGame.SetActive(level1Done);
        }
    }

    /// <summary>Chơi nhanh từ Màn 1 (giữ nguyên để backward compatible)</summary>
    public void PlayGame()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.currentLevelIndex = 0;
        SceneManager.LoadScene("Level");
    }

    /// <summary>Chơi Mới - xóa toàn bộ tiến trình, về Màn 1, vào LevelSelect</summary>
    public void OnNewGameButton()
    {
        // Clear all saves: levels are locked again, high scores are reset
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.ResetAllProgress();

        // Go to Level Select: only Level 1 will be unlocked
        SceneManager.LoadScene("LevelSelect");
    }

    /// <summary>Mở màn hình chọn màn chơi</summary>
    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
