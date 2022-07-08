using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHitEvent : UnityEvent<GameObject> {}

public class PositionHitEvent : UnityEvent<Vector3> {}

public class Tower : MonoBehaviour {
  public TowerType towerType;
  
  private float currentShotCooldown = 0.0f;
  
  private List<Collider> enemiesInRange;
  private Collider currentTarget = null;
  
  private Vector3 launchOrigin;
  
  void Start() {
    enemiesInRange = new List<Collider>();
    currentShotCooldown = towerType.shotCooldown;
    
    CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
    
    capsuleCollider.radius = towerType.range;
    capsuleCollider.height = towerType.range * 4.0f;
    
    launchOrigin = transform.position;
    
    foreach (Transform child in transform) {
      if (child.CompareTag(TagConstants.LAUNCHORIGIN)) {
        launchOrigin = child.position;
        break;
      }
    }
  }

  void Update() {
    if (currentShotCooldown > 0.0f) {
      currentShotCooldown -= Time.deltaTime;
    } else {
      UpdateTarget();
      
      if (currentTarget != null) {
        ShootAtEnemy();
        currentShotCooldown = towerType.shotCooldown;
      }
    }
  }
  
  private void ShootAtEnemy() {
    if (towerType.projectilePrefab != null) {
      GameObject projectileObject = Instantiate(towerType.projectilePrefab, launchOrigin, Quaternion.identity);
      Projectile projectile = projectileObject.GetComponent<Projectile>();
      
      if (towerType.projectileTracksEnemy) {
        projectile.SetTargetObject(currentTarget.gameObject);
      } else {
        projectile.SetTargetPosition(currentTarget.gameObject.transform.position);
      }
      
      projectile.SetSpeed(towerType.projectileSpeed);
      
      EnemyHitEvent enemyHitEvent = new EnemyHitEvent();
      enemyHitEvent.AddListener(OnEnemyHit);
      
      PositionHitEvent positionHitEvent = new PositionHitEvent();
      positionHitEvent.AddListener(OnPositionHit);
      
      projectile.SetEnemyHitEvent(enemyHitEvent);
      projectile.SetPositionHitEvent(positionHitEvent);
    }
  }
  
  private void UpdateTarget() {
    PurgeDestroyedEnemies();
    
    switch(towerType.targetingMethod) {
      case TowerType.TargetingMethod.HighestPriority:
        currentTarget = FindHighestPriorityEnemy();
        break;
      case TowerType.TargetingMethod.LowestPriority:
        currentTarget = FindLowestPriorityEnemy();
        break;
      case TowerType.TargetingMethod.Closest:
        currentTarget = FindClosestEnemy();
        break;
      default:
        currentTarget = null;
        break;
    }
  }
  
  private Collider FindHighestPriorityEnemy() {
    Collider target = null;
    
    foreach (Collider collider in enemiesInRange) {
      EnemyStats enemyStats = collider.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null) {
        if (target == null) {
          target = collider;
        } else {
          EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
          
          if (targetStats != null && targetStats.GetPriority() < enemyStats.GetPriority()) {
            target = collider;
          }
        }
      }
    }
    
    return target;
  }
  
  private Collider FindLowestPriorityEnemy() {
    Collider target = null;
    
    foreach (Collider collider in enemiesInRange) {
      EnemyStats enemyStats = collider.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null) {
        if (target == null) {
          target = collider;
        } else {
          EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
          
          if (targetStats != null && targetStats.GetPriority() > enemyStats.GetPriority()) {
            target = collider;
          }
        }
      }
    }
    
    return target;
  }
  
  private Collider FindClosestEnemy() {
    Collider target = null;
    
    foreach (Collider collider in enemiesInRange) {
      EnemyStats enemyStats = collider.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null) {
        if (target == null) {
          target = collider;
        } else {
          EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
          
          if (targetStats != null && Vector3.Distance(transform.position, collider.gameObject.transform.position) < Vector3.Distance(transform.position, target.gameObject.transform.position)) {
            target = collider;
          }
        }
      }
    }
    
    return target;
  }
  
  private void PurgeDestroyedEnemies() {
    for (int i = enemiesInRange.Count - 1; i >= 0; i--) {
      if (enemiesInRange[i] == null) {
        enemiesInRange.RemoveAt(i);
      }
    }
  }
  
  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag(TagConstants.ENEMY) && !enemiesInRange.Contains(other)) {
      enemiesInRange.Add(other);
    }
  }
  
  private void OnTriggerExit(Collider other) {
    if (other.CompareTag(TagConstants.ENEMY) && enemiesInRange.Contains(other)) {
      enemiesInRange.Remove(other);
    }
  }
  
  private void OnEnemyHit(GameObject enemy) {
    if (towerType.areaEffectPrefab != null) {
      Instantiate(towerType.areaEffectPrefab, enemy.transform.position, Quaternion.identity);
    }
    
    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
      
    if (enemyStats != null) {
      enemyStats.RemoveHealth(towerType.damagePerShot);
    }
  }
  
  private void OnPositionHit(Vector3 position) {
    if (towerType.areaEffectPrefab != null) {
      Instantiate(towerType.areaEffectPrefab, position, Quaternion.identity);
    }
  }
}
