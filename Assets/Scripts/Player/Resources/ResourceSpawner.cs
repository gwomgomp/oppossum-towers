using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour {

    [field: SerializeField]
    public ResourceType TypeToSpawn { get; private set; }

    [field: SerializeField]
    public List<ResourceSpawnLocation> ResourceLocations { get; set; } = new List<ResourceSpawnLocation>();

    private float timeSinceLastSpawn = 0f;
    private int amountSpawned = 0;
    private bool spawning = false;

    public void Update() {
        if (spawning &&
            ResourceLocations.HasFreeSpawnSpots() &&
            amountSpawned < TypeToSpawn.SpawnCap &&
            timeSinceLastSpawn >= TypeToSpawn.SpawnCooldown) {

            var freeSpawnSpots = ResourceLocations.FindAll(location => location.IsFree);
            var resourceLocation = freeSpawnSpots[Random.Range(0, freeSpawnSpots.Count)];
            var resourceGameObject = Instantiate(TypeToSpawn.Prefab, resourceLocation.transform.position, Quaternion.LookRotation(transform.position, Vector3.up));
            var resource = resourceGameObject.GetComponent<Resource>();
            resource.Initialize(TypeToSpawn, resourceLocation);

            timeSinceLastSpawn = 0f;
            amountSpawned++;
        } else {
            timeSinceLastSpawn += Time.deltaTime;
        }
    }

    public void ResourceTaken() {
        amountSpawned -= 1;
    }

    internal void StartSpawning() {
        spawning = true;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

}
