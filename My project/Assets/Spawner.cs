using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public List<Spawnable> spawnables = new List<Spawnable>();
    public Collider2D triggerArea; // Reference to the trigger collider
    public float minSpacing = 1f; // Minimum distance between obstacles

    void Start()
    {
        spawnables.ForEach(spawnable => SpawnObstacles(spawnable));
    }

    void SpawnObstacles(Spawnable spawnable)
    {
        if (triggerArea == null || spawnable.spawnablePrefab == null)
        {
            Debug.LogError("Trigger Area or Obstacle Prefab is not assigned!");
            return;
        }

        // Get the bounds of the trigger area
        Bounds bounds = triggerArea.bounds;

        List<Vector2> spawnPositions = new List<Vector2>();

        for (int i = 0; i < spawnable.numOfItems; i++)
        {
            Vector2 spawnPosition;
            int attempts = 0;

            // Try to find a random position far enough from other obstacles
            do
            {
                float xPosition = Random.Range(bounds.min.x, bounds.max.x);
                float yPosition = Random.Range(bounds.min.y, bounds.max.y);
                spawnPosition = new Vector2(xPosition, yPosition);
                attempts++;
            }
            while (!IsFarEnough(spawnPosition, spawnPositions, minSpacing) && attempts < 100);

            // Add the position to the list of spawned positions
            spawnPositions.Add(spawnPosition);

            // Instantiate the obstacle
            GameObject obstacle = Instantiate(spawnable.spawnablePrefab, spawnPosition, Quaternion.identity);

            // Apply random rotation if enabled
            if (spawnable.shouldRandomRotation)
            {
                float randomRotation = Random.Range(0f, 360f);
                obstacle.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
            }

            // Apply random scale
            float randomScale = Random.Range(spawnable.minScale, spawnable.maxScale);
            obstacle.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }
    }

    // Check if the position is far enough from existing spawn positions
    bool IsFarEnough(Vector2 position, List<Vector2> existingPositions, float minDistance)
    {
        foreach (Vector2 existingPosition in existingPositions)
        {
            if (Vector2.Distance(position, existingPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    [System.Serializable]
    public struct Spawnable
    {
        public GameObject spawnablePrefab;
        public int numOfItems;

        public bool shouldRandomRotation; // Whether to apply random rotation
        public float minScale, maxScale;  // Min and max scale for randomization
    }
}
