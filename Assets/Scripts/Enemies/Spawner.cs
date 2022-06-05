using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField]
    private EnemyType typeToSpawn;
    [SerializeField]
    private float timeBetweenSpawns;
    [SerializeField]
    private int amountToSpawn;

    [SerializeField]
    private LaneCheckpoint firstCheckpoint;

    private float timeSinceLastSpawn = 0f;
    private int amountSpawned = 0;
    private bool spawning = false;

    public void Update() {
        if (spawning && amountSpawned < amountToSpawn && timeSinceLastSpawn >= timeBetweenSpawns) {
            var enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
            var enemyGameObject = Instantiate(enemyPrefab, transform.position, Quaternion.LookRotation(firstCheckpoint.transform.position, Vector3.up));
            var enemy = enemyGameObject.GetComponent<Enemy>();
            enemy.Initialize(typeToSpawn, firstCheckpoint);
            timeSinceLastSpawn = 0f;
            amountSpawned++;
        } else {
            timeSinceLastSpawn += Time.deltaTime;
        }
    }

    public LaneCheckpoint GetFirstCheckpoint() {
        return firstCheckpoint;
    }

    public LaneCheckpoint GetLastCheckpoint() {
        HashSet<LaneCheckpoint> handledCheckpoints = new();
        var currentCheckPoint = firstCheckpoint;
        while (currentCheckPoint != null && !handledCheckpoints.Contains(currentCheckPoint)) {
            handledCheckpoints.Add(currentCheckPoint);
            var nextCheckpoint = currentCheckPoint.NextCheckpoint;
            if (nextCheckpoint == null || handledCheckpoints.Contains(nextCheckpoint)) {
                return currentCheckPoint;
            } else {
                currentCheckPoint = nextCheckpoint;
            }
        }
        return null;
    }

    internal void StartSpawning() {
        spawning = true;
    }

    public void OnDrawGizmosSelected() {
        HashSet<LaneCheckpoint> handledCheckpoints = new();
        var currentCheckPoint = firstCheckpoint;
        if (currentCheckPoint != null) {
            Gizmos.DrawLine(transform.position, currentCheckPoint.transform.position);
        }

        while (currentCheckPoint != null && !handledCheckpoints.Contains(currentCheckPoint)) {
            handledCheckpoints.Add(currentCheckPoint);
            var nextCheckpoint = currentCheckPoint.NextCheckpoint;
            if (nextCheckpoint != null) {
                Gizmos.DrawLine(currentCheckPoint.transform.position, nextCheckpoint.transform.position);
            }
            currentCheckPoint = nextCheckpoint;
        }
    }
}
