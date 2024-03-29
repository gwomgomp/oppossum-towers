using System;
using UnityEngine;

[Serializable]
public struct WaveDefinition<T> {
    [field: SerializeField]
    public int RoundToSpawnIn { get; private set; }
    [field: SerializeField]
    public T TypeToSpawn { get; private set; }
    [field: SerializeField]
    public float TimeBetweenSpawns { get; private set; }
    [field: SerializeField]
    public float TimeBeforeSpawns { get; private set; }
    [field: SerializeField]
    public int AmountToSpawn { get; private set; }
}

public enum SpawnStatus {
    FINISHED,
    READY,
    WAITING
}

public class Wave {
    private WaveDefinition<EnemyType> Definition;

    private float timeSinceLastSpawn = 0f;
    private int amountSpawned = 0;

    public Wave(WaveDefinition<EnemyType> definition) {
        Definition = definition;
    }

    public void Update(float timePassed) {
        timeSinceLastSpawn += timePassed;
    }

    public void Spawn(GameObject enemyGameObject, LaneCheckpoint firstCheckPoint) {
        timeSinceLastSpawn = 0f;
        amountSpawned++;

        var enemy = enemyGameObject.RequireComponent<Enemy>();
        enemy.Initialize(Definition.TypeToSpawn, firstCheckPoint);
    }

    public SpawnStatus GetSpawnStatus() {
        if (amountSpawned >= Definition.AmountToSpawn) {
            return SpawnStatus.FINISHED;
        } else if (timeSinceLastSpawn >= Definition.TimeBetweenSpawns + (amountSpawned == 0 ? Definition.TimeBeforeSpawns : 0)) {
            return SpawnStatus.READY;
        } else {
            return SpawnStatus.WAITING;
        }
    }
}
