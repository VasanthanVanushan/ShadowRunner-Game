
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Environment Prefabs")]
    [SerializeField] private GameObject[] environments;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 800f;
    [SerializeField] private float destroyDelay = 2f;


    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned)
            return;

        if (!other.CompareTag("Player"))
            return;

        SpawnEnvironment();

        Destroy(transform.root.gameObject, destroyDelay);
    }

    private void SpawnEnvironment()
    {
        if (environments == null || environments.Length == 0)
        {
            Debug.LogWarning("No environments assigned!");
            return;
        }

        // Select a random environment
        int randomIndex = Random.Range(0, environments.Length);
        GameObject selectedEnvironment = environments[randomIndex];

        if (selectedEnvironment == null)
        {
            Debug.LogWarning("Selected environment prefab is missing!");
            return;
        }

    
        Vector3 spawnPosition = new Vector3(0f,0f,transform.position.z + spawnDistance);

        // Preserve the prefab's rotation
        Quaternion spawnRotation = selectedEnvironment.transform.rotation;

        Instantiate(
            selectedEnvironment,
            spawnPosition,
            spawnRotation
        );

        hasSpawned = true;

        Debug.Log("Environment spawned: " + selectedEnvironment.name);
    }
}
