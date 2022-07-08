using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower Type", menuName = "Towers/Tower Type", order = 1)]
public class TowerType : ScriptableObject {
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
  public GameObject areaEffectPrefab;
}
