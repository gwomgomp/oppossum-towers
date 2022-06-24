using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
  private bool trackEnemy = true;
  private Vector3 targetPosition = Vector3.zero;
  private GameObject targetGameObject = null;
  
  private float speed = 10.0f;
  
  private EnemyHitEvent enemyHitEvent = null;
  private PositionHitEvent positionHitEvent = null;
  
  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {
    float step = speed * Time.deltaTime;
    
    if (trackEnemy)
    {
      if (targetGameObject != null)
      {
        targetPosition = targetGameObject.transform.position;
      }
      else
      {
        trackEnemy = false;
      }
    }
    
    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    
    if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
    {
      if (trackEnemy && enemyHitEvent != null)
      {
        enemyHitEvent.Invoke(targetGameObject);
        Destroy(gameObject);
      }
      else if (positionHitEvent != null)
      {
        positionHitEvent.Invoke(targetPosition);
        Destroy(gameObject);
      }
    }
    
    Vector3 lookDirection = targetPosition - transform.position;
    transform.rotation = Quaternion.LookRotation(lookDirection);
  }
  
  public void SetTargetPosition(Vector3 targetPosition)
  {
    this.targetPosition = targetPosition;
    trackEnemy = false;
  }
  
  public void SetTargetGameObject(GameObject targetGameObject)
  {
    this.targetGameObject = targetGameObject;
    trackEnemy = true;
  }
  
  public void SetSpeed(float speed)
  {
    this.speed = speed;
  }
  
  public void SetEnemyHitEvent(EnemyHitEvent enemyHitEvent)
  {
    this.enemyHitEvent = enemyHitEvent;
  }
  
  public void SetPositionHitEvent(PositionHitEvent positionHitEvent)
  {
    this.positionHitEvent = positionHitEvent;
  }
}
