using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour {
    [SerializeField]
    private WaveDefinition[] waves;

    private List<Wave> currentWaves;

    [field: SerializeField]
    public LaneCheckpoint FirstCheckpoint { get; set; }

    public delegate void SpawnFinished();
    public event SpawnFinished OnSpawningFinished;

    private bool finishedRound = false;

    public void Start() {
        PlayerIgnoreCollisionHelper.IgnorePlayerCollision(gameObject);
    }

    public void Update() {
        if (currentWaves == null) {
            return;
        }

        foreach (var wave in currentWaves) {
            switch (wave.GetSpawnStatus()) {
                case SpawnStatus.READY: {
                        var enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
                        var enemyGameObject = Instantiate(
                            enemyPrefab,
                            transform.position,
                            Quaternion.LookRotation(FirstCheckpoint.transform.position, Vector3.up)
                        );
                        wave.Spawn(enemyGameObject, FirstCheckpoint);
                        break;
                    }
                case SpawnStatus.WAITING:
                    wave.Update(Time.deltaTime);
                    break;
            }
        }

        currentWaves.RemoveAll(wave => wave.GetSpawnStatus() == SpawnStatus.FINISHED);
        if (!finishedRound && currentWaves.Count == 0) {
            finishedRound = true;
            OnSpawningFinished();
        }
    }

    public void PrepareNewRound(int roundNumber) {
        currentWaves = waves.Where(wave => wave.RoundToSpawnIn == roundNumber).Select(wave => new Wave(wave)).ToList();
        if (currentWaves.Count > 0) {
            finishedRound = false;
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
