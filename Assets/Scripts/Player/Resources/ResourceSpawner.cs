using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour {
    public float resourceDisplayHeight;
    public Material resourceDisplayMaterial;

    [field: SerializeField]
    public List<ResourceSpawnLocation> ResourceLocations { get; set; } = new List<ResourceSpawnLocation>();

    [SerializeField]
    private WaveDefinition<ResourceType>[] waves;

    private WaveDefinition<ResourceType> currentWave;

    private float timeSinceLastSpawn = 0f;
    private int amountSpawned = 0;
    private bool spawning = false;

    private GameObject resourceDisplay;

    public void Start() {
        var wave = FindWaveDefinitionForRound(0);
        if (wave.Equals(null)) {
            clearResourceDisplay();
        } else {
            setupResourceDisplay(wave);
        }
    }

    public void Update() {

        if (!spawning ||
            amountSpawned >= currentWave.AmountToSpawn) return;

        if (ResourceLocations.HasFreeSpawnSpots()
            && timeSinceLastSpawn >= currentWave.TimeBetweenSpawns) {

            var resourceLocation = ResourceLocations.Find(location => location.IsFree());
            var resourceGameObject = Instantiate(
                currentWave.TypeToSpawn.Prefab,
                resourceLocation.transform.position,
                Quaternion.LookRotation(Vector3.forward, Vector3.up),
                resourceLocation.transform
            );
            var resource = resourceGameObject.RequireComponentInChildren<Resource>();
            resource.Initialize(currentWave.TypeToSpawn, resourceLocation, this);

            timeSinceLastSpawn = 0f;
            amountSpawned++;
        } else {
            timeSinceLastSpawn += Time.deltaTime;
        }
    }

    public void ReturnResource() {
        amountSpawned -= 1;
    }

    /// <summary>
    /// start spawn of resources
    /// </summary>
    /// <param name="roundNumber">Round which should be prepared</param>
    internal void PrepareNewRound(int roundNumber) {
        var wave = FindWaveDefinitionForRound(roundNumber);
        if (wave.Equals(null)) {
            clearResourceDisplay();
            return;
        }
        spawning = true;
        currentWave = wave;
        setupResourceDisplay(wave);
    }

    private WaveDefinition<ResourceType> FindWaveDefinitionForRound(int roundNumber) {
        return waves.OrderByDescending(wave => wave.RoundToSpawnIn).FirstOrDefault(wave => wave.RoundToSpawnIn <= roundNumber);
    }

    private void setupResourceDisplay(WaveDefinition<ResourceType> wave) {
        clearResourceDisplay();
        resourceDisplay = Instantiate(wave.TypeToSpawn.Prefab, transform.position + Vector3.up * resourceDisplayHeight, Quaternion.identity, transform);
        Destroy(resourceDisplay.GetComponent<Resource>());
        Destroy(resourceDisplay.GetComponentInChildren<Interactable>().gameObject);
        resourceDisplay.transform.localScale = Vector3.one * 0.5f;
        // at this point the destroyed components above are not yet cleaned up, so we still find the interactable renderer
        var renderers = resourceDisplay.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers) {
            renderer.material = resourceDisplayMaterial;
        }
    }

    private void clearResourceDisplay() {
        Destroy(resourceDisplay);
    }

    internal void StopSpawning() {
        spawning = false;
        timeSinceLastSpawn = 0f;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one);
    }
}
