using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static bool gameOver;
    public GameObject gameOverPanel;

    public static bool isGameStarted;
    public GameObject startingText;

    public static int numberOfCoins;
    public int timeOfGame;
    public float timer;

    public Text coinsText;
    public Text timeText;
    public Text speedText;
    public Text distanceText;   // Displays remaining distance to the finish line

    public int speed;
    private float totalDistance; // Total distance to the finish line (Unity units ≈ meters)

    [Header("Điều khiển UI Hamburger")]
    public GameObject hamburgerButton;   // Hamburger icon pause button (three horizontal lines)

    private static string playerName;
    bool alreadyDone = false;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        timeOfGame = 0;

        speed = 0;

        gameOver = false;
        Time.timeScale = 1;
        isGameStarted = false;
        numberOfCoins = 0;
        PauseManager.isPaused = false;

        // Get total distance to finish from TileManager
        TileManager tm = FindObjectOfType<TileManager>();
        if (tm != null)
            totalDistance = tm.finishAfterTiles * tm.tileLength;
        else if (ProgressManager.Instance != null)
            totalDistance = LevelData.DistanceMeters[ProgressManager.Instance.currentLevelIndex];
        else
            totalDistance = 500f; // fallback

        // Hide hamburger button initially (game not started yet)
        if (hamburgerButton != null)
            hamburgerButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        UpdateSpeed();

        if (gameOver)
        {
            Time.timeScale = 0;
            // Hide hamburger button on game over
            if (hamburgerButton != null)
                hamburgerButton.SetActive(false);
            if (!alreadyDone)
            {
                Events eventsObject = FindObjectOfType<Events>();
                eventsObject.UnhideScoreSaving();
                alreadyDone = true;
            }
        }
        else if (isGameStarted)
        {
            // Show hamburger button only while actively playing
            if (hamburgerButton != null && !PauseManager.isPaused)
                hamburgerButton.SetActive(true);
        }

        coinsText.text = "Coins: " + numberOfCoins;
        timeText.text = "Time: " + FormatTimeText();
        speedText.text = "Speed: " + FormatSpeedText();
        UpdateDistance();

        StartCoroutine(StartGame());
    }

    void UpdateTime()
    {
        if (isGameStarted)
        {
            timer += Time.deltaTime;
            timeOfGame = Convert.ToInt32(timer);
        }
    }

    void UpdateDistance()
    {
        if (distanceText == null) return;

        if (!isGameStarted)
        {
            distanceText.text = "Đến đích: " + Mathf.CeilToInt(totalDistance) + "m";
            return;
        }

        GameObject player = GameObject.Find("Player");
        if (player == null) return;

        float traveled = player.transform.position.z;
        int remaining  = Mathf.Max(0, Mathf.CeilToInt(totalDistance - traveled));
        distanceText.text = "Còn: " + remaining + "m";
    }

    void UpdateSpeed()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                speed = Convert.ToInt32(controller.displayedSpeed);
            }
        }
    }

    string FormatSpeedText()
    {
        Color orange = new Color(1.0f, 0.64f, 0.0f);

        switch (speed)
        {
            case int s when (s < 220 && s >= 150):
                speedText.color = orange;
                speedText.fontSize = 55;
                break;

            case int s when (s <= 300 && s >= 220):
                speedText.color = Color.red;
                speedText.fontSize = 65;
                break;

            default:
                break;
        }

        return string.Format("{0} km/h", speed);
    }

    string FormatTimeText()
    {
        return (timeOfGame.ToString()).PadLeft(3, ' ') + "s";
    }

    private IEnumerator StartGame()
    {
        if (SwipeManager.tap)
        {
            if (!isGameStarted)
            {
                var am = FindObjectOfType<AudioManager>();
                StartCoroutine(AudioManager.FadeOut(am.GetComponent<AudioSource>(), 1, 0.2f));
                am.PlaySound("StartingUp");

                yield return new WaitForSeconds(1);
                
                isGameStarted = true;

                Destroy(startingText);
            }
        }
    }

    public static string getPlayerName()
    {
        return playerName;
    }

    public static void SetPlayerName(string _playerName)
    {
        playerName = _playerName;
    }
}
