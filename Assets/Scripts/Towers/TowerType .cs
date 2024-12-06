using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerType : ScriptableObject {
    public string displayName;

    public enum TargetingMethod {
        HighestPriority,
        LowestPriority,
        Closest
    }

    public TargetingMethod targetingMethod;

    public DamageType damageType;
    public float damagePerShot;
    public float rampUpMultiplier;
    public float rampUpShotsNeeded;
    public float shotCooldown;
    public float range;
    public float targetCount;
    public bool attackAllInRange;
    public bool turnTowardsTarget;

    public bool projectileTracksEnemy;
    public float projectileSpeed;

    public GameObject projectilePrefab;
    public StatusEffect statusEffect;
    public GameObject areaEffectPrefab;

    public ResourceCost[] buildCosts;
    public List<TowerUpgradeType> upgrades;

    [Serializable]
    public struct ResourceCost {
        public ResourceType resourceType;
        public int count;
    }
}
