using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip gameBGM;
    [SerializeField] private AudioClip gameOverSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup BGM
        bgmSource.clip = gameBGM;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        // Start background music
        bgmSource.Play();

        // Setup SFX
        sfxSource.playOnAwake = false;
    }

    public void GameOver()
    {
        // Stop background music
        bgmSource.Stop();

        // Play game-over sound
        if (gameOverSFX != null)
        {
            sfxSource.PlayOneShot(gameOverSFX);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}