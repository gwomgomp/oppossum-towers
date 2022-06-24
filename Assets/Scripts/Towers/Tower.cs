using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHitEvent : UnityEvent<GameObject>
{
}

public class PositionHitEvent : UnityEvent<Vector3>
{
}

public class Tower : MonoBehaviour
{
  public enum TargetingMethod
  {
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
  public GameObject groundEffectPrefab;
  
  private float currentShotCooldown = 0.0f;
  
  private List<Collider> enemiesInRange;
  private Collider currentTarget = null;
  
  private Vector3 launchOrigin;
  
  void Start()
  {
    enemiesInRange = new List<Collider>();
    currentShotCooldown = shotCooldown;
    
    CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
    
    capsuleCollider.radius = range;
    capsuleCollider.height = range * 4.0f;
    
    launchOrigin = transform.position;
    
    foreach (Transform child in transform)
    {
      if (child.tag == "LaunchOrigin")
      {
        launchOrigin = child.position;
        break;
      }
    }
  }

  void Update()
  {
    if (enemiesInRange.Count > 0)
    {
      SelectNewTarget();
    }
    
    UpdateShotCooldown();
  }
  
  private void UpdateShotCooldown()
  {
    if (currentShotCooldown <= 0.0f && currentTarget != null)
    {
      ShootAtEnemy();
      currentShotCooldown = shotCooldown;
    }
    else
    {
      currentShotCooldown -= Time.deltaTime;
    }
  }
  
  private void ShootAtEnemy()
  {
    if (projectilePrefab != null)
    {
      GameObject projectileObject = Instantiate(projectilePrefab, launchOrigin, Quaternion.identity);
      Projectile projectile = projectileObject.GetComponent<Projectile>();
      
      if (projectileTracksEnemy)
      {
        projectile.SetTargetGameObject(currentTarget.gameObject);
      }
      else
      {
        projectile.SetTargetPosition(currentTarget.gameObject.transform.position);
      }
      
      projectile.SetSpeed(projectileSpeed);
      
      EnemyHitEvent enemyHitEvent = new EnemyHitEvent();
      enemyHitEvent.AddListener(OnEnemyHit);
      
      PositionHitEvent positionHitEvent = new PositionHitEvent();
      positionHitEvent.AddListener(OnPositionHit);
      
      projectile.SetEnemyHitEvent(enemyHitEvent);
      projectile.SetPositionHitEvent(positionHitEvent);
    }
  }
  
  private void SelectNewTarget()
  {
    PurgeDestroyedEnemies();
    
    switch(targetingMethod)
    {
      case TargetingMethod.HighestPriority:
        currentTarget = FindHighestPriorityEnemy();
        break;
      case TargetingMethod.LowestPriority:
        currentTarget = FindLowestPriorityEnemy();
        break;
      case TargetingMethod.Closest:
        currentTarget = FindClosestEnemy();
        break;
      default:
        currentTarget = null;
        break;
    }
  }
  
  private Collider FindHighestPriorityEnemy()
  {
    Collider target = null;
    
    foreach (Collider collider in enemiesInRange)
    {
      EnemyStats enemyStats = collider.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null)
      {
        if (target == null)
        {
          target = collider;
        }
        else
        {
          EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
          
          if (targetStats != null && targetStats.GetPriority() < enemyStats.GetPriority())
          {
            target = collider;
          }
        }
      }
    }
    
    // This is purely for visualization purposes (i.e. serves no functional purpose)
    // From here
    if (currentTarget != target)
    {
      if (currentTarget != null)
      {
        EnemyStats targetStats = currentTarget.gameObject.GetComponent<EnemyStats>();
        
        if (targetStats != null)
        {
          targetStats.BeUntargeted();
        }
      }
      
      if (target != null)
      {
        EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
        
        if (targetStats != null)
        {
          targetStats.BeTargeted();
        }
      }
    }
    // To here
    
    return target;
  }
  
  private Collider FindLowestPriorityEnemy()
  {
    Collider target = null;
    
    foreach (Collider collider in enemiesInRange)
    {
      EnemyStats enemyStats = collider.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null)
      {
        if (target == null)
        {
          target = collider;
        }
        else
        {
          EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
          
          if (targetStats != null && targetStats.GetPriority() > enemyStats.GetPriority())
          {
            target = collider;
          }
        }
      }
    }
    
    // This is purely for visualization purposes (i.e. serves no functional purpose)
    // From here
    if (currentTarget != target)
    {
      if (currentTarget != null)
      {
        EnemyStats targetStats = currentTarget.gameObject.GetComponent<EnemyStats>();
        
        if (targetStats != null)
        {
          targetStats.BeUntargeted();
        }
      }
      
      if (target != null)
      {
        EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
        
        if (targetStats != null)
        {
          targetStats.BeTargeted();
        }
      }
    }
    // To here
    
    return target;
  }
  
  private Collider FindClosestEnemy()
  {
    Collider target = null;
    
    foreach (Collider collider in enemiesInRange)
    {
      EnemyStats enemyStats = collider.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null)
      {
        if (target == null)
        {
          target = collider;
        }
        else
        {
          EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
          
          if (targetStats != null && Vector3.Distance(transform.position, collider.gameObject.transform.position) < Vector3.Distance(transform.position, target.gameObject.transform.position))
          {
            target = collider;
          }
        }
      }
    }
    
    // This is purely for visualization purposes (i.e. serves no functional purpose)
    // From here
    if (currentTarget != target)
    {
      if (currentTarget != null)
      {
        EnemyStats targetStats = currentTarget.gameObject.GetComponent<EnemyStats>();
        
        if (targetStats != null)
        {
          targetStats.BeUntargeted();
        }
      }
      
      if (target != null)
      {
        EnemyStats targetStats = target.gameObject.GetComponent<EnemyStats>();
        
        if (targetStats != null)
        {
          targetStats.BeTargeted();
        }
      }
    }
    // To here
    
    return target;
  }
  
  private void PurgeDestroyedEnemies()
  {
    for (int i = enemiesInRange.Count - 1; i >= 0; i--)
    {
      if (enemiesInRange[i] == null)
      {
        enemiesInRange.RemoveAt(i);
      }
    }
  }
  
  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Enemy" && !enemiesInRange.Contains(other))
    {
      enemiesInRange.Add(other);
    }
    
    SelectNewTarget();
  }
  
  private void OnTriggerExit(Collider other)
  {
    if (other.tag == "Enemy" && enemiesInRange.Contains(other))
    {
      enemiesInRange.Remove(other);
    }
    
    SelectNewTarget();
  }
  
  private void OnEnemyHit(GameObject enemy)
  {
    if (groundEffectPrefab != null)
    {
      Instantiate(groundEffectPrefab, enemy.transform.position, Quaternion.identity);
    }
    
    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
      
    if (enemyStats != null)
    {
      enemyStats.RemoveHealth(damagePerShot);
    }
  }
  
  private void OnPositionHit(Vector3 position)
  {
    if (groundEffectPrefab != null)
    {
      Instantiate(groundEffectPrefab, position, Quaternion.identity);
    }
  }
}
