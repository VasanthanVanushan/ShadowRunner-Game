
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private TMP_Text activeScoreText;

    private float startZ;
    private int currentScore;
    private int highScore;

    private const string HighScoreKey = "ShadowRunnerHighScore";

    public int CurrentScore => currentScore;
    public int HighScore => highScore;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            startZ = player.position.z;
        }

        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        UpdateScoreUI();
    }

    private void Update()
    {
        if (player == null)
            return;

        // Calculate score based on the distance travelled.
        currentScore = Mathf.Max(0,Mathf.FloorToInt(player.position.z - startZ));

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
        {
            activeScoreText.text = "Score: " + currentScore;
        }
    }
}
