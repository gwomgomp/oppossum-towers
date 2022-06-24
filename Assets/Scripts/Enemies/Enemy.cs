using UnityEngine;

public class Enemy : MonoBehaviour {
    public float health;

    private const float rotationSpeed = 5f;
    private const float moveDampening = 5f;

    private bool initialized = false;
    private EnemyType type = null;
    private LaneCheckpoint currentTarget = null;

    private Vector3 velocity = Vector3.zero;

    private bool carryingLoot = false;
    private GameObject loot = null;

    public void Initialize(EnemyType type, LaneCheckpoint startingCheckpoint) {
        if (!initialized) {
            this.type = type;
            health = type.MaxHealth;
            currentTarget = startingCheckpoint;
            transform.rotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
            initialized = true;
        } else {
            Debug.LogError("Do not try to initialize enemy twice");
        }
    }

    public void Update() {
        if (initialized && currentTarget != null) {
            Move();
        }
    }

    private void Move() {
        var newPosition = Vector3.SmoothDamp(
                transform.position,
                currentTarget.transform.position,
                ref velocity,
                CalculateExpectedTravelTime(),
                type.Speed
            );
        transform.position = newPosition;

        var targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collider) {
        if (initialized && collider.gameObject.TryGetComponent(out LaneCheckpoint next) && next == currentTarget) {
            HandleHoardCheckpoint(next);
            HandleDropOffCheckpoint(next);
            HandleLastCheckpoint(next);
        }
    }

    private void HandleHoardCheckpoint(LaneCheckpoint checkpoint) {
        Hoard hoard = checkpoint as Hoard;
        if (hoard != null && hoard.TakeLoot()) {
            carryingLoot = true;
            var checkpointPrefab = Resources.Load<GameObject>("Prefabs/Loot");
            loot = Instantiate(checkpointPrefab, transform);
        }
    }

    private void HandleDropOffCheckpoint(LaneCheckpoint checkpoint) {
        if (carryingLoot && checkpoint.IsLootDropOff) {
            Debug.Log("Extracted loot");
            Destroy(loot);
            carryingLoot = false;
        }
    }

    private void HandleLastCheckpoint(LaneCheckpoint checkpoint) {
        if (checkpoint.NextCheckpoint == null) {
            Destroy(gameObject);
        } else {
            currentTarget = checkpoint.NextCheckpoint;
        }
    }

    private float CalculateExpectedTravelTime() {
        if (currentTarget != null) {
            var distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            return distance / type.Speed / moveDampening;
        } else {
            return 0f;
        }
    }
}
