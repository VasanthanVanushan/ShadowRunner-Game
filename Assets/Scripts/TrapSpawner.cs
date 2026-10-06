using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    [Header("Trap Prefabs")]
    [SerializeField] private List<GameObject> traps = new List<GameObject>();

    [Header("Spawn Settings")]
    [SerializeField] private float startZ = 20f;
    [SerializeField] private float endZ = 200f;

    [Header("Distance Between Traps")]
    [SerializeField] private float minDistance = 10f;
    [SerializeField] private float maxDistance = 25f;

    [Header("Switch Offset")]
    [SerializeField] private float switchXOffset = 1.5f;

    private float currentZ;

    private void Start()
    {
        SpawnTraps();
    }

    private void SpawnTraps()
    {
        currentZ = startZ;

        while (currentZ <= endZ)
        {
            // Select a random trap from the list
            GameObject selectedTrap = traps[Random.Range(0, traps.Count)];

            // Get the prefab's position
            Vector3 spawnPosition = selectedTrap.transform.position;

            // Keep X and Y from the prefab
            // Only change Z
            spawnPosition.z = currentZ;

            // Keep the exact prefab rotation
            Quaternion spawnRotation = selectedTrap.transform.rotation;

            // Spawn the trap
            GameObject spawnedTrap = Instantiate(selectedTrap,spawnPosition,spawnRotation,transform);

            // Find Switch inside the spawned trap
            Transform switchTransform = spawnedTrap.transform.Find("Switch");
            if (switchTransform != null)
            {
                Vector3 switchPosition = switchTransform.localPosition;
                switchPosition.x += switchXOffset;
                switchTransform.localPosition = switchPosition;
            }

            // Random distance before the next trap
            float randomDistance = Random.Range(minDistance, maxDistance);

            currentZ += randomDistance;
        }
    }
}