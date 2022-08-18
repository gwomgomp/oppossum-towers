using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public float health;
    public float baseSpeed;
    public float modifiedSpeed;
    
    private float damagePerSecondReceiving;

    private const float rotationSpeed = 5f;
    private const float moveDampening = 5f;

    private bool initialized = false;
    private EnemyType type = null;
    private LaneCheckpoint currentTarget = null;

    private Vector3 velocity = Vector3.zero;

    private bool carryingLoot = false;
    private GameObject currentLoot = null;
  
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

    private void OnTriggerEnter(Collider collider) {
        if (initialized && collider.gameObject.TryGetComponent(out LaneCheckpoint next) && next == currentTarget) {
            HandleHoardCheckpoint(next);
            HandleDropOffCheckpoint(next);
            HandleLastCheckpoint(next);
            return;
        }

        if (initialized && !carryingLoot && collider.CompareTag(TagConstants.LOOT)) {
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
  
    public void ApplyStatusEffect(StatusEffect statusEffect) {
        activeStatusEffects.Add(statusEffect);
        
        RefreshStatusEffects();
    }
    
    public void RemoveStatusEffect(StatusEffect statusEffect) {
        activeStatusEffects.Remove(statusEffect);
        
        RefreshStatusEffects();
    }
    
    public void ApplyTimedStatusEffect(StatusEffect statusEffect, Tower originTower)
    {
        TimedStatusEffect newTimedStatusEffect = new TimedStatusEffect(statusEffect, originTower);
        
        bool statusEffectMissing = true;
        
        foreach (TimedStatusEffect timedStatusEffect in timedStatusEffects) {
            if (newTimedStatusEffect.Equals(timedStatusEffect)) {
                timedStatusEffect.RefreshTimer();
                statusEffectMissing = false;
            }
        }
        
        if (statusEffectMissing) {
            timedStatusEffects.Add(newTimedStatusEffect);
        }
        
        RefreshStatusEffects();
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
    
    private void UpdateTimedStatusEffects() {
        List<int> toBeDeleted = new List<int>();
        
        for (int i = 0; i < timedStatusEffects.Count; i++) {
            TimedStatusEffect timedStatusEffect = timedStatusEffects[i];
            
            if(timedStatusEffect.HasEnded()) {
                toBeDeleted.Add(i);
            } else {
                timedStatusEffect.UpdateTimer(Time.deltaTime);
            }
        }
        
        for (int i = toBeDeleted.Count - 1; i >= 0; i--){
            timedStatusEffects.RemoveAt(toBeDeleted[i]);
        }
        
        if (toBeDeleted.Count > 0) {
            RefreshStatusEffects();
        }
    }

    public int GetPriority() {
        return type.Priority;
    }
}

public class TimedStatusEffect {
    public StatusEffect statusEffect;
    public Tower originTower;
    
    private float timer = 0.0f;
    
    public TimedStatusEffect(StatusEffect statusEffect, Tower originTower) {
        this.statusEffect = statusEffect;
        this.originTower = originTower;
    }
    
    public void UpdateTimer(float timeDelta) {
        timer += timeDelta;
    }
    
    public bool HasEnded() {
        if (timer >= statusEffect.duration) {
            return true;
        }
        
        return false;
    }
    
    public bool Equals(TimedStatusEffect timedStatusEffect) {
        if (timedStatusEffect.statusEffect == this.statusEffect && timedStatusEffect.originTower == this.originTower)
        {
            return true;
        }
        
        return false;
    }
    
    public void RefreshTimer() {
        timer = 0.0f;
    }
}
