using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour {
  private AreaEffectType areaEffectType = null;
  
  private float timer = 0;
  
  private List<EnemyStats> enemiesInArea;
  
  void Start() {
    enemiesInArea = new List<EnemyStats>();
    
    if (areaEffectType.snapToGround) {
      // Ground level should be defined by the level in the future via global variable
      transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
  }

  void Update() {
    timer += Time.deltaTime;
    
    if (timer >= areaEffectType.duration) {
      foreach (EnemyStats enemy in enemiesInArea) {
        enemy.RemoveStatusEffect(areaEffectType.statusEffect);
      }
      
      Destroy(gameObject);
    }
  }
  
  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag(TagConstants.ENEMY)) {
      EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null) {
        enemiesInArea.Add(enemyStats);
        
        enemyStats.ApplyStatusEffect(areaEffectType.statusEffect);
      }
    }
  }
  
  private void OnTriggerExit(Collider other) {
    if (other.CompareTag(TagConstants.ENEMY)) {
      EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
      
      if (enemyStats != null) {
        enemiesInArea.Remove(enemyStats);
        
        enemyStats.RemoveStatusEffect(areaEffectType.statusEffect);
      }
    }
  }
  
  public void SetAreaEffectType(AreaEffectType areaEffectType) {
    this.areaEffectType = areaEffectType;
  }
}
