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

        if (!spawning) return;

        if (ResourceLocations.HasFreeSpawnSpots()
            && amountSpawned < TypeToSpawn.SpawnCap
            && timeSinceLastSpawn >= TypeToSpawn.SpawnCooldown) {

            var resourceLocation = ResourceLocations.Find(location => location.IsFree());
            var resourceGameObject = Instantiate(
                TypeToSpawn.Prefab,
                resourceLocation.transform.position,
                Quaternion.LookRotation(Vector3.forward, Vector3.up),
                resourceLocation.transform
            );
            var resource = resourceGameObject.RequireComponentInChildren<Resource>();
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

    /// <summary>
    /// start spawn of resources
    /// TODO: base something of the resources spawned on roundnumber
    /// </summary>
    /// <param name="roundNumber"></param>
    internal void StartSpawning(int roundNumber) {
        spawning = true;
    }

    internal void StopSpawning() {
        spawning = false;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
