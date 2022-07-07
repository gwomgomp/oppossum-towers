using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField]
    private ResourceType typeToSpawn;
    [SerializeField]
    private float timeBetweenSpawns;
    [SerializeField]
    private int amountToSpawn;

    [field: SerializeField]
    public ResourceLocation ResourceLocation { get; set; }

    private float timeSinceLastSpawn = 0f;
    private int amountSpawned = 0;
    private bool spawning = false;

    public void Update() {
        if (spawning && amountSpawned < amountToSpawn && timeSinceLastSpawn >= timeBetweenSpawns) {
            var resourcePrefab = Resources.Load<GameObject>("Prefabs/Resource");
            var resourceGameObject = Instantiate(resourcePrefab, transform.position, Quaternion.LookRotation(ResourceLocation.transform.position, Vector3.up));
            var resource = resourceGameObject.GetComponent<Resource>();
            resource.Initialize(typeToSpawn, ResourceLocation);
            timeSinceLastSpawn = 0f;
            amountSpawned++;
        } else {
            timeSinceLastSpawn += Time.deltaTime;
        }
    }

    internal void StartSpawning() {
        spawning = true;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

    public void OnDrawGizmosSelected() {
        DrawLaneGizmos();
    }

    private void DrawLaneGizmos() {
  
    }
}
