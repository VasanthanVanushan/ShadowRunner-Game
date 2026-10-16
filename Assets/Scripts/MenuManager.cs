
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    public static MenuManagerScript Instance;

    [Header("Victory Scene References")]
    [SerializeField] private VictoryCameraController victoryCamera;
    [SerializeField] private GameObject victoryMenu;

    [Header("Victory Settings")]
    [SerializeField] private float cameraMovementDuration = 5f;

    private Coroutine victoryRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Victory presentation only runs in Scene 2.
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (victoryMenu != null)
            {
                victoryMenu.SetActive(false);
            }

            if (victoryCamera != null)
            {
                victoryCamera.SetOrbitEnabled(true);
            }

            victoryRoutine = StartCoroutine(ShowVictoryMenu());
        }
    }

    private IEnumerator ShowVictoryMenu()
    {
        yield return new WaitForSeconds(cameraMovementDuration);

        if (victoryCamera != null)
        {
            victoryCamera.SetOrbitEnabled(false);
        }

        if (victoryMenu != null)
        {
            victoryMenu.SetActive(true);
        }

        victoryRoutine = null;
    }

    // Scene 0: Start button
    public void StartGame()
    {
        LoadScene(1);
    }

    // Any scene: Home button
    public void MainMenu()
    {
        LoadScene(0);
    }

    // Scene 1: Restart button
    public void RestartGame()
    {
        LoadScene(1);
    }

    // Scene 2: Play Again button
    public void PlayAgain()
    {
        LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1f;

        if (victoryRoutine != null)
        {
            StopCoroutine(victoryRoutine);
            victoryRoutine = null;
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
