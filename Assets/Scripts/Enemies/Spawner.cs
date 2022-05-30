using System;
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
    [SerializeField]
    private GameObject enemyPrefab;

    private float timeSinceLastSpawn = 0f;
    private int amountSpawned = 0;
    private bool spawning = false;

    public void Update() {
        if (spawning && amountSpawned < amountToSpawn && timeSinceLastSpawn >= timeBetweenSpawns) {
            GameObject enemyGO = Instantiate(enemyPrefab, transform.position, Quaternion.LookRotation(firstCheckpoint.transform.position, Vector3.up));
            Enemy enemy = enemyGO.GetComponent<Enemy>();
            enemy.Initialize(typeToSpawn, firstCheckpoint);
            timeSinceLastSpawn = 0f;
            amountSpawned++;
        } else {
            timeSinceLastSpawn += Time.deltaTime;
        }
    }

    internal void StartSpawning() {
        spawning = true;
    }
}
