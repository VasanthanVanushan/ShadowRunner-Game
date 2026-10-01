using UnityEngine;

public class TrapSwitch : MonoBehaviour
{
    [Header("Trap")]
    [SerializeField] private GameObject trapParent;

    [Header("Switch Settings")]
    [SerializeField] private bool disableParticles = true;
    [SerializeField] private bool disableAnimators = true;
    [SerializeField] private bool disableColliders = true;

    private bool isActivated = false;

    [Header("Puzzle")]
    private SequencePuzzle sequencePuzzle;


    private void Awake()
    {
        trapParent = transform.parent.gameObject;
    }

    private void Start()
    {
        sequencePuzzle = FindFirstObjectByType<SequencePuzzle>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated)
            return;

        if (!other.CompareTag("Player"))
            return;


        Time.timeScale = 0f;

        sequencePuzzle.StartPuzzle(OnPuzzleFinished);
    }


    private void OnPuzzleFinished(bool success)
    {
        if (success)
        {
            TurnOffTrap();
        }

        Time.timeScale = 1f;
    }

    private void TurnOffTrap()
    {
        if (trapParent == null)
        {
            Debug.LogWarning("Trap Parent is not assigned to Trap Switch.");
            return;
        }

        isActivated = true;

        // Find all objects inside the trap parent
        Transform[] children = trapParent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            // Disable Particle Systems
            if (disableParticles)
            {
                ParticleSystem particle = child.GetComponent<ParticleSystem>();

                if (particle != null)
                {
                    particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    particle.gameObject.SetActive(false);
                }
            }

            // Handle Animators
            if (disableAnimators)
            {
                Animator animator = child.GetComponent<Animator>();

                if (animator != null)
                {
                    if (trapParent.CompareTag("SpikeTrap") || trapParent.CompareTag("JumpTrap"))
                    {
                        // Enable animator and reset to its initial state
                        animator.enabled = true;
                        animator.Rebind();
                        animator.Update(0f);
                        animator.enabled = false;

                    }
                    else if (trapParent.CompareTag("PressTrap"))
                    {
                       // Enable animator
                        animator.enabled = true;
                        animator.speed = 1f;

                        // Move to 50% of the animation
                        animator.Play(0, 0, 0.5f);
                        animator.Update(0f);

                        // Stop at 50%
                        animator.speed = 0f;
                        animator.enabled = false;
                    }
                    else
                    {
                        // Disable animator for other traps
                        animator.enabled = false;
                    }
                }
            }
            
            if (disableColliders)
            {
                Collider collider = child.GetComponent<Collider>();

                if (collider != null && child.name.Contains("TriggerArea"))
                {
                    collider.enabled = false;
                }
            }
        }

        Debug.Log("Trap disabled: " + trapParent.name);
    }
}