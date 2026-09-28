using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    [SerializeField] private GameObject[] obstacles;

    [Header("Spawn Settings")]
    [SerializeField] private float startZ = 20f;
    [SerializeField] private float endZ = 200f;

    [Header("Distance Between Obstacles")]
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 15f;

    [Header("Horizontal Position")]
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;

    [Header("Floor")]
    [SerializeField] private float floorY = 0f;

    private void Start()
    {
        SpawnObstacles();
    }

    private void SpawnObstacles()
    {
        float currentZ = startZ;

        while (currentZ <= endZ)
        {
            SpawnObstacle(currentZ);

            // Random distance to the next obstacle
            float randomDistance = Random.Range(minDistance, maxDistance);

            currentZ += randomDistance;
        }
    }

    private void SpawnObstacle(float zPosition)
    {
        // Check if obstacles are assigned
        if (obstacles == null || obstacles.Length == 0)
        {
            Debug.LogWarning("No obstacles assigned to the ObstacleSpawner.");
            return;
        }

        // Select a random obstacle
        int randomIndex = Random.Range(0, obstacles.Length);

        // Select a random horizontal position
        float randomX = Random.Range(minX, maxX);

        Vector3 spawnPosition = new Vector3(randomX,floorY,zPosition);

        // Spawn obstacle as a child of this GameObject
        GameObject spawnedObstacle = Instantiate(obstacles[randomIndex],spawnPosition,Quaternion.identity,transform);

        // Find collider on the obstacle or its children
        Collider obstacleCollider = spawnedObstacle.GetComponentInChildren<Collider>();

        if (obstacleCollider != null)
        {
            // Find how far the bottom of the collider is from the floor
            float bottomOffset = obstacleCollider.bounds.min.y - floorY;

            // Move obstacle vertically so its bottom touches the floor
            spawnedObstacle.transform.position -= new Vector3(0f,bottomOffset,0f);
        }
        else
        {
            Debug.LogWarning("No Collider found on obstacle: " + spawnedObstacle.name);
        }
    }
}