
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VictoryCameraController victoryCamera;
    [SerializeField] private GameObject victoryMenu;

    [Header("Settings")]
    [SerializeField] private float cameraMovementDuration = 5f;

    private IEnumerator Start()
    {
        // Hide the menu when the scene starts.
        if (victoryMenu != null)
        {
            victoryMenu.SetActive(false);
        }

        // Allow the camera to orbit during the victory dance.
        if (victoryCamera != null)
        {
            victoryCamera.SetOrbitEnabled(true);
        }

        // Wait for the camera movement duration.
        yield return new WaitForSeconds(cameraMovementDuration);

        // Stop the camera orbit.
        if (victoryCamera != null)
        {
            victoryCamera.SetOrbitEnabled(false);
        }

        // Show the victory menu.
        if (victoryMenu != null)
        {
            victoryMenu.SetActive(true);
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
