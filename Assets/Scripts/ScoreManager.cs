
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private TMP_Text activeScoreText;
    [SerializeField] private TMP_Text gameoverscoreText;

    private float startZ;
    private int currentScore;
    private int highScore;
    private int initialHighScore;

    private const string HighScoreKey = "ShadowRunnerHighScore";

    public int CurrentScore => currentScore;
    public int HighScore => highScore;
    public int InitialHighScore => initialHighScore;

    private void Awake()
    {
        // Load the saved high score before gameplay starts.
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        // Preserve the record from the beginning of this run.
        initialHighScore = highScore;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player != null)
            startZ = player.position.z;

        currentScore = 0;
        UpdateScoreUI();
    }

    private void Update()
    {
        if (player == null)
            return;

        // Calculate the score from distance travelled.
        currentScore = Mathf.Max(
            0,
            Mathf.FloorToInt(player.position.z - startZ)
        );

        // Save a new record, without changing InitialHighScore.
        if (currentScore > highScore)
        {
            highScore = currentScore;

            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (activeScoreText != null)
            activeScoreText.text = "Score: " + currentScore;

        if (gameoverscoreText != null)
            gameoverscoreText.text = currentScore.ToString();
    }
}
