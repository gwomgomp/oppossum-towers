using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour {

    [field: SerializeField]
    public ResourceType TypeToSpawn { get; private set; }

    [SerializeField]
    private float _timeBetweenSpawns;

    [SerializeField]
    private int _amountToSpawn;

    [field: SerializeField]
    public List<ResourceSpawnLocation> ResourceLocations { get; set; } = new List<ResourceSpawnLocation>();

    private float _timeSinceLastSpawn = 0f;
    private int _amountSpawned = 0;
    private bool _spawning = false;

    public void Update() {
        if (_spawning &&
            ResourceLocations.HasFreeSpawnSpots() &&
            _amountSpawned < _amountToSpawn &&
            _timeSinceLastSpawn >= _timeBetweenSpawns) {
            var resourceLocation = ResourceLocations.FindAll(location => !location.IsResourcePlaced)[Random.Range(0, ResourceLocations.Count)];
            var resourcePrefab = Resources.Load<GameObject>($"Prefabs/Resources/{TypeToSpawn.name}");
            var resourceGameObject = Instantiate(resourcePrefab, resourceLocation.transform.position, Quaternion.LookRotation(transform.position, Vector3.up));
            var resource = resourceGameObject.GetComponent<Resource>();
            resource.Initialize(TypeToSpawn, resourceLocation);

            _timeSinceLastSpawn = 0f;
            _amountSpawned++;
        } else {
            _timeSinceLastSpawn += Time.deltaTime;
        }
    }

    public void ResourceTaken() {
        _amountSpawned -= 1;
        _timeSinceLastSpawn = 0f;
    }

    internal void StartSpawning() {
        _spawning = true;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

}
