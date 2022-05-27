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
}
