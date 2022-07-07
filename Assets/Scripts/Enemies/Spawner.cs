using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField]
    private EnemyType typeToSpawn;
    [SerializeField]
    private float timeBetweenSpawns;
    [SerializeField]
    private int amountToSpawn;

    [field: SerializeField]
    public LaneCheckpoint FirstCheckpoint { get; set; }

    private float timeSinceLastSpawn = 0f;
    private int amountSpawned = 0;
    private bool spawning = false;

    public void Update() {
        if (spawning && amountSpawned < amountToSpawn && timeSinceLastSpawn >= timeBetweenSpawns) {
            var enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
            var enemyGameObject = Instantiate(enemyPrefab, transform.position, Quaternion.LookRotation(FirstCheckpoint.transform.position, Vector3.up));
            var enemy = enemyGameObject.GetComponent<Enemy>();
            enemy.Initialize(typeToSpawn, FirstCheckpoint);
            timeSinceLastSpawn = 0f;
            amountSpawned++;
        } else {
            timeSinceLastSpawn += Time.deltaTime;
        }
    }

    public (LaneCheckpoint, int, int) GetLastCheckpoint() {
        HashSet<LaneCheckpoint> handledCheckpoints = new();
        var currentCheckPoint = FirstCheckpoint;
        var count = 0;
        var hoardCount = 0;
        while (currentCheckPoint != null && !handledCheckpoints.Contains(currentCheckPoint)) {
            count++;
            if (currentCheckPoint is Hoard) {
                hoardCount++;
            }
            handledCheckpoints.Add(currentCheckPoint);
            var nextCheckpoint = currentCheckPoint.NextCheckpoint;
            if (nextCheckpoint == null || handledCheckpoints.Contains(nextCheckpoint)) {
                return (currentCheckPoint, count - hoardCount, hoardCount);
            } else {
                currentCheckPoint = nextCheckpoint;
            }
        }
        return (null, 0, 0);
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
        HashSet<LaneCheckpoint> handledCheckpoints = new();
        var currentCheckPoint = FirstCheckpoint;
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
