using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour {
  public float duration;
  public bool snapToGround;
  
  public StatusEffect statusEffect;
  
  private float timer = 0;
  
  private List<EnemyStats> enemiesInArea;
  
  void Start() {
    enemiesInArea = new List<EnemyStats>();
    
    if (snapToGround) {
      // Ground level should be defined by the level in the future via global variable
      transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
  }

  void Update() {
    timer += Time.deltaTime;
    
    if (timer >= duration) {
      foreach (EnemyStats enemy in enemiesInArea) {
        enemy.RemoveStatusEffect(statusEffect);
      }
      
      Destroy(gameObject);
    }
  }
  
  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag(TagConstants.ENEMY)) {
      EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null) {
        enemiesInArea.Add(enemyStats);
        
        enemyStats.ApplyStatusEffect(statusEffect);
      }
    }
  }
  
  private void OnTriggerExit(Collider other) {
    if (other.CompareTag(TagConstants.ENEMY)) {
      EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null) {
        enemiesInArea.Remove(enemyStats);
        
        enemyStats.RemoveStatusEffect(statusEffect);
      }
    }
  }
}
