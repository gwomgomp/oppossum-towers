using UnityEngine;

public class RoundManager : MonoBehaviour {
    public static RoundManager Instance { get; private set; }

    public delegate void NewRound(int roundNumber);
    public event NewRound OnNextRound;

    public bool Running { get; private set; }

    public float timeBetweenRounds = 0f;

    private int roundNumber = 0;
    private int spawnerCount = 0;
    private int finishedSpawners = 0;

    private bool roundFinished = false;
    private float timeSinceRoundEnd = 0f;

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        Spawner[] spawners = FindObjectsOfType<Spawner>();
        spawnerCount = spawners.Length;
        foreach (var spawner in spawners)
        {
            OnNextRound += spawner.PrepareNewRound;
            spawner.OnSpawningFinished += HandleFinishedSpawner;
        }
    }

    public void Update() {
        if (roundFinished) {
            timeSinceRoundEnd += Time.deltaTime;
            if (timeSinceRoundEnd >= timeBetweenRounds) {
                timeSinceRoundEnd = 0f;
                roundFinished = false;
                StartNewRound();
            }
        }
    }

    public void Run() {
        Running = true;
        StartNewRound();
    }

    private void HandleFinishedSpawner() {
        finishedSpawners++;
        if (finishedSpawners == spawnerCount) {
            FinishRound();
        }
    }

    private void FinishRound() {
        Debug.Log(string.Format("Round {0} finished.", roundNumber));
        roundFinished = true;
    }

    private void StartNewRound() {
        finishedSpawners = 0;
        roundNumber++;
        OnNextRound(roundNumber);
        Debug.Log(string.Format("Round {0} started.", roundNumber));
    }
}
