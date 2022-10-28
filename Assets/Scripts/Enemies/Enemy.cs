using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Enemy : MonoBehaviour {
    public float health;
    private float baseSpeed;
    private float modifiedSpeed;

    private float damagePerSecondReceiving;

    private const float rotationSpeed = 5f;
    private const float moveDampening = 5f;

    private bool initialized = false;
    private EnemyType type = null;
    private LaneCheckpoint currentTarget = null;

    private Vector3 velocity = Vector3.zero;

    private bool carryingLoot = false;
    private Loot currentLoot = null;

    private List<StatusEffect> activeStatusEffects;
    private List<TimedStatusEffect> timedStatusEffects;

    void Start() {
        activeStatusEffects = new List<StatusEffect>();
        timedStatusEffects = new List<TimedStatusEffect>();
    }

    public void Initialize(EnemyType type, LaneCheckpoint startingCheckpoint) {
        if (!initialized) {
            this.type = type;
            health = type.MaxHealth;
            baseSpeed = type.Speed;
            modifiedSpeed = type.Speed;
            currentTarget = startingCheckpoint;
            transform.rotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
            initialized = true;
        } else {
            Debug.LogError("Do not try to initialize enemy twice");
        }
    }

    public void Update() {
        UpdateTimedStatusEffects();
        Damage(damagePerSecondReceiving * Time.deltaTime);

        if (initialized && currentTarget != null) {
            Move();
        }
    }

    public void HandleCheckpoint(LaneCheckpoint checkpoint) {
        if (checkpoint == currentTarget) {
            HandleHoardCheckpoint(checkpoint);
            HandleDropOffCheckpoint(checkpoint);
            HandleLastCheckpoint(checkpoint);
        }
    }

    public void HandleLoot(Loot loot) {
        if (!carryingLoot) {
            AttachLootToTransform(loot);
        }
    }

    private void Move() {
        var newPosition = Vector3.SmoothDamp(
                transform.position,
                currentTarget.transform.position,
                ref velocity,
                CalculateExpectedTravelTime(),
                modifiedSpeed
            );
        transform.position = newPosition;

        var targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleHoardCheckpoint(LaneCheckpoint checkpoint) {
        Hoard hoard = checkpoint as Hoard;
        if (!carryingLoot && hoard != null && hoard.TakeLoot()) {
            var lootPrefab = Resources.Load<GameObject>("Prefabs/Loot");
            var loot = Instantiate(lootPrefab);
            AttachLootToTransform(loot.GetComponent<Loot>());
        }
    }

    private void AttachLootToTransform(Loot loot) {
        currentLoot = loot;
        carryingLoot = true;
        loot.AttachToTransform(transform);
    }

    private void DetachLootFromTransform(Loot loot) {
        currentLoot = null;
        carryingLoot = false;
        loot.DetachFromTransform();
    }

    private void HandleDropOffCheckpoint(LaneCheckpoint checkpoint) {
        if (carryingLoot && checkpoint.IsLootDropOff) {
            Debug.Log("Extracted loot");
            Loot toExtract = currentLoot;
            DetachLootFromTransform(toExtract);
            Destroy(toExtract.gameObject);
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

    public void ApplyStatusEffect(StatusEffect statusEffect) {
        activeStatusEffects.Add(statusEffect);

        RefreshStatusEffects();
    }

    public void RemoveStatusEffect(StatusEffect statusEffect) {
        activeStatusEffects.Remove(statusEffect);

        RefreshStatusEffects();
    }

    public void ApplyTimedStatusEffect(StatusEffect statusEffect, Tower originTower) {
        TimedStatusEffect newEffect = new(statusEffect, originTower);

        var effectToUpdate = timedStatusEffects.Where(effect => effect.Equals(newEffect)).FirstOrDefault(null);
        if (effectToUpdate == null) {
            timedStatusEffects.Add(newEffect);
        } else {
            effectToUpdate.RefreshTimer();
        }

        RefreshStatusEffects();
    }

    private void UpdateTimedStatusEffects() {
        int removedEffects = timedStatusEffects.RemoveAll(effect => effect.HasEnded());
        timedStatusEffects.ForEach(effect => effect.UpdateTimer(Time.deltaTime));

        if (removedEffects > 0) {
            RefreshStatusEffects();
        }
    }

    private void RefreshStatusEffects() {
        float maxSlowPercentage = 0.0f;
        float totalDamagePerSecond = 0.0f;

        foreach (StatusEffect statusEffect in activeStatusEffects) {
            if (statusEffect.slowPercentage > maxSlowPercentage) {
                maxSlowPercentage = statusEffect.slowPercentage;
            }

            totalDamagePerSecond += statusEffect.damagePerSecond;
        }

        foreach (TimedStatusEffect timedStatusEffect in timedStatusEffects) {
            if (timedStatusEffect.statusEffect.slowPercentage > maxSlowPercentage) {
                maxSlowPercentage = timedStatusEffect.statusEffect.slowPercentage;
            }

            totalDamagePerSecond += timedStatusEffect.statusEffect.damagePerSecond;
        }

        modifiedSpeed = baseSpeed * ((100.0f - maxSlowPercentage) / 100.0f);
        damagePerSecondReceiving = totalDamagePerSecond;
    }

    public int GetPriority() {
        return type.Priority;
    }
}
