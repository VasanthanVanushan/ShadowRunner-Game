
using UnityEngine;

public class TrapProximityAudio : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Trap Audio")]
    [SerializeField] private AudioClip trapAudioClip;

    [Header("Detection")]
    [SerializeField] private float detectionDistance = 50f;

    private bool isAudioPlaying;

     private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' was not found!");
        }
    }

    private void Update()
    {
        if (player == null || trapAudioClip == null)
            return;

        float distance = Vector3.Distance(player.position,transform.position);

        bool playerIsNear = distance <= detectionDistance;

        if (playerIsNear && !isAudioPlaying)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayTrapAudio(trapAudioClip);
                isAudioPlaying = true;
            }
        }
        else if (!playerIsNear && isAudioPlaying)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopTrapAudio(trapAudioClip);
            }

            isAudioPlaying = false;
        }
    }

    private void OnDisable()
    {
        if (isAudioPlaying && AudioManager.Instance != null)
        {
            AudioManager.Instance.StopTrapAudio(trapAudioClip);
        }

        isAudioPlaying = false;
    }
}
