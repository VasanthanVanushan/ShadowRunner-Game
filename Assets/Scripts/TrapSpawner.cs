using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    [Header("Trap Prefabs")]
    [SerializeField] private List<GameObject> traps = new List<GameObject>();

    [Header("Distance Between Traps")]
    [SerializeField] private float minDistance = 10f;
    [SerializeField] private float maxDistance = 25f;

    [Header("Switch Offset")]
    [SerializeField] private float switchXOffset = 1.5f;

    private float currentZ;

    [Header("Land Z Range")]
    [SerializeField] private Transform landStart;
    [SerializeField] private Transform landEnd;

    private void Start()
    {
        SpawnTraps();
    }

    private void SpawnTraps()
    {
        if (traps == null || traps.Count == 0)
        {
            Debug.LogWarning("No traps assigned to TrapSpawner.");
            return;
        }

        if (landStart == null || landEnd == null)
        {
            Debug.LogWarning(
                "Assign Land Start and Land End in the Inspector."
            );
            return;
        }

        float startZ = landStart.position.z;
        float endZ = landEnd.position.z;

        if (endZ < startZ)
        {
            Debug.LogWarning("Land End must be after Land Start.");
            return;
        }

        currentZ = startZ;

        while (currentZ <= endZ)
        {
            // Select a random trap
            GameObject selectedTrap = traps[Random.Range(0, traps.Count)];

            if (selectedTrap != null)
            {
                // Keep prefab X and Y; change only Z
                Vector3 spawnPosition = selectedTrap.transform.position;

                spawnPosition.z = currentZ;

                // Preserve prefab rotation
                Quaternion spawnRotation =
                    selectedTrap.transform.rotation;

                // Spawn trap as a child of this spawner
                GameObject spawnedTrap = Instantiate(selectedTrap,spawnPosition,spawnRotation,transform);

                // Find and adjust the Switch
                Transform switchTransform =spawnedTrap.transform.Find("Switch");

                if (switchTransform != null)
                {
                    Vector3 switchPosition =switchTransform.localPosition;

                    switchPosition.x += switchXOffset;

                    switchTransform.localPosition =switchPosition;
                }
            }

            // Random distance to the next trap
            currentZ += Random.Range(minDistance, maxDistance);
        }
    }
}