using System;
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
    private GameObject currentLoot = null;

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
            return;
        }

        if (initialized && !carryingLoot && collider.CompareTag("Loot")) {
            AttachLootToTransform(collider.gameObject);
            return;
        }
    }

    private void HandleHoardCheckpoint(LaneCheckpoint checkpoint) {
        Hoard hoard = checkpoint as Hoard;
        if (!carryingLoot && hoard != null && hoard.TakeLoot()) {
            var checkpointPrefab = Resources.Load<GameObject>("Prefabs/Loot");
            var loot = Instantiate(checkpointPrefab);
            AttachLootToTransform(loot);
        }
    }

    private void AttachLootToTransform(GameObject loot) {
        currentLoot = loot;
        carryingLoot = true;
        loot.TryGetComponent(out Collider collider);
        collider.enabled = false;
        loot.transform.SetParent(transform);
        loot.transform.position = transform.position;
        loot.transform.Translate(Vector3.up * 2, Space.World);
    }

    private void DetachLootFromTransform(GameObject loot) {
        currentLoot = null;
        carryingLoot = false;
        loot.TryGetComponent(out Collider collider);
        collider.enabled = true;
        loot.transform.SetParent(transform.parent);
        loot.transform.position = transform.position;
    }

    private void HandleDropOffCheckpoint(LaneCheckpoint checkpoint) {
        if (carryingLoot && checkpoint.IsLootDropOff) {
            Debug.Log("Extracted loot");
            DetachLootFromTransform(currentLoot);
            Destroy(currentLoot);
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

    public bool Damage(float damage) {
        health -= damage;

        if (health <= 0.0f) {
            if (carryingLoot) {
                DetachLootFromTransform(currentLoot);
            }
            Destroy(gameObject);
            return true;
        } else {
            return false;
        }
    }

    public int GetPriority() {
        return type.Priority;
    }

    // These two methods are purely for visualization purposes (i.e. serve no functional purpose)
    public void BeTargeted() {
        GetComponent<Renderer>().material = type.TargetMaterial;
    }

    public void BeUntargeted() {
        GetComponent<Renderer>().material = type.DefaultMaterial;
    }
}
