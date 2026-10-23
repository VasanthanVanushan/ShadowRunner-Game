
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button pauseButton;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    private bool isPaused = false;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        pauseButton.interactable = true;
    }

    private void Update()
    {
        // Disable pause button when Game Over panel is active
        if (gameOverPanel.activeInHierarchy)
        {
            pauseButton.interactable = false;

            // Close pause menu if the game ends
            if (isPaused)
            {
                pauseMenuUI.SetActive(false);
                isPaused = false;
                Time.timeScale = 1f;
            }
        }
        else
        {
            pauseButton.interactable = true;
        }
    }

    public void PauseGame()
    {
        if (gameOverPanel.activeInHierarchy)
            return;

        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
}
