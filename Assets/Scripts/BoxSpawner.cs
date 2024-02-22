using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    public GameObject modelPrefab; // Reference to the prefab of the model you want to spawn
    public float moveSpeed = 5f; // Speed at which the spawned models move in the negative x direction
    public float destroyRelativePositionX = -12.623f; // Relative X position at which to destroy and respawn the objects
    public float minSpawnDelay = 2f; // Minimum delay before spawning a new object
    public float maxSpawnDelay = 5f; // Maximum delay before spawning a new object
    public int maxSpawnedModels = 10; // Maximum number of spawned models to keep in memory
    [Range(0.0f, 1.0f)]
    public float spawnRotationChance; // Chance to spawn a rotated box

    [Range(0.0f, 1.0f)]
    public float spawnErrorChance; // Chance to spawn a rotated box
    public Axis rotationAxis = Axis.Y;
    public Vector3 localOffset = new Vector3(0f, 0f, 0.1f);

    private List<GameObject> spawnedModels = new List<GameObject>(); // List to keep track of spawned models
    private float nextSpawnTime; // Time at which the next object should be spawned

    private void Start()
    {
        // Initialize the next spawn time
        nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);

        // Spawn the initial models
        SpawnModel();
    }

    private void SpawnModel()
    {
        // Instantiate the modelPrefab and set it as a child of the current GameObject
        GameObject newModel = Instantiate(modelPrefab, transform);

        // Set the position of the spawned model relative to the current GameObject
        newModel.transform.localPosition = localOffset;

        // Check if the model should be rotated
        if (Random.value <= spawnRotationChance)
        {
            // Apply a rotation on the y-axis
            newModel.transform.Rotate(new Vector3(rotationAxis == Axis.X ? 90f : 0f, rotationAxis == Axis.Y ? 90f : 0f, rotationAxis == Axis.Z ? 90f : 0f));
        }
        if (Random.value <= spawnErrorChance)
        {
            // Apply a rotation on the y-axis
            newModel.transform.Rotate(new Vector3(rotationAxis == Axis.X ? Random.value * 360 : 0f, rotationAxis == Axis.Y ? Random.value * 360 : 0f, rotationAxis == Axis.Z ? Random.value * 360 : 0f));
        }

        // Add the spawned model to the list
        spawnedModels.Add(newModel);
    }

    private void Update()
    {
        // Move all spawned models in the negative x direction
        foreach (var model in spawnedModels)
        {
            model.transform.localPosition += Vector3.left * moveSpeed * Time.deltaTime;
        }

        // Check if any spawned models exceed the destroy position
        for (int i = spawnedModels.Count - 1; i >= 0; i--)
        {
            if (spawnedModels[i].transform.localPosition.x <= destroyRelativePositionX)
            {
                // Destroy and remove the model that exceeds the destroy position
                Destroy(spawnedModels[i]);
                spawnedModels.RemoveAt(i);
            }
        }

        // Check if it's time to spawn a new object
        if (Time.time >= nextSpawnTime && spawnedModels.Count < maxSpawnedModels)
        {
            // Spawn a new model
            SpawnModel();

            // Set the next spawn time after a random delay
            nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }
}
