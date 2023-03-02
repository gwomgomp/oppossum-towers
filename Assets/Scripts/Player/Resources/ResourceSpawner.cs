using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour {


    [field: SerializeField]
    public List<ResourceSpawnLocation> ResourceLocations { get; set; } = new List<ResourceSpawnLocation>();

    [SerializeField]
    private WaveDefinition<ResourceType>[] waves;

    private WaveDefinition<ResourceType> currentWave;

    private float timeSinceLastSpawn = 0f;
    private int amountSpawned = 0;
    private bool spawning = false;

    public void Update() {

        if (!spawning) return;

        if (ResourceLocations.HasFreeSpawnSpots()
            && amountSpawned < currentWave.AmountToSpawn
            && timeSinceLastSpawn >= currentWave.TimeBetweenSpawns) {

            var resourceLocation = ResourceLocations.Find(location => location.IsFree());
            var resourceGameObject = Instantiate(
                currentWave.TypeToSpawn.Prefab,
                resourceLocation.transform.position,
                Quaternion.LookRotation(Vector3.forward, Vector3.up),
                resourceLocation.transform
            );
            var resource = resourceGameObject.RequireComponentInChildren<Resource>();
            resource.Initialize(currentWave.TypeToSpawn, resourceLocation);

            timeSinceLastSpawn = 0f;
            amountSpawned++;
        } else {
            timeSinceLastSpawn += Time.deltaTime;
        }
    }

    public void ResourceTaken() {
        amountSpawned -= 1;
    }

    /// <summary>
    /// start spawn of resources
    /// </summary>
    /// <param name="roundNumber">Round which should be prepared</param>
    internal void PrepareNewRound(int roundNumber) {
        var wave = waves.OrderByDescending(wave => wave.RoundToSpawnIn).FirstOrDefault(wave => wave.RoundToSpawnIn <= roundNumber);
        if (wave.Equals(null)) {
            return;
        }
        spawning = true;
        currentWave = wave;
    }

    internal void StopSpawning() {
        spawning = false;
        timeSinceLastSpawn = 0f;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
