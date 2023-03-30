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

    public float damagePerShot;
    public float shotCooldown;
    public float range;

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
