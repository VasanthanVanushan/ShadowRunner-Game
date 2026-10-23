
using TMPro;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;

    private const string HighScoreKey = "ShadowRunnerHighScore";

    private void Start()
    {
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (highScoreText != null)
        {
            highScoreText.text = " "+highScore;
        }
    }
}
