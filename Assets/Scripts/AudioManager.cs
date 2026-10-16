
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Scene Background Music")]
    [SerializeField] private AudioClip mainMenuBGM; // Scene 0
    [SerializeField] private AudioClip gameplayBGM; // Scene 1
    [SerializeField] private AudioClip victoryBGM;  // Scene 2

    [Header("Sound Effects")]
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioClip buttonClickSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource.playOnAwake = false;
        bgmSource.loop = true;

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    private void OnEnable()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.buildIndex);
    }

    private void PlayMusicForScene(int sceneIndex)
    {
        AudioClip selectedMusic = null;

        switch (sceneIndex)
        {
            case 0:
                selectedMusic = mainMenuBGM;
                break;

            case 1:
                selectedMusic = gameplayBGM;
                break;

            case 2:
                selectedMusic = victoryBGM;
                break;
        }

        bgmSource.Stop();
        bgmSource.clip = selectedMusic;

        if (selectedMusic != null)
        {
            bgmSource.Play();
        }
    }

    public void PlayGameOverSFX()
    {
        bgmSource.Stop();

        if (gameOverSFX != null)
        {
            sfxSource.PlayOneShot(gameOverSFX);
        }
    }

    public void PlayButtonClickSFX()
    {
        if (buttonClickSFX != null)
        {
            sfxSource.PlayOneShot(buttonClickSFX);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StopMusic()
    {
        bgmSource.Stop();
    }

    public void RestartMusic()
    {
        if (bgmSource.clip != null)
        {
            bgmSource.Stop();
            bgmSource.Play();
        }
    }
}
