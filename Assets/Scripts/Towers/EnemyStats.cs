using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Temporary helper class for tower targeting implementation, to be replaced with actual enemy class later
public class EnemyStats : MonoBehaviour {
  public float maxHealth;
  public float speed;
  // Arbitrary enemy priority to test different targeting methods
  public int priority;
  
  public Material defaultMaterial;
  public Material damagedMaterial;
  public Material nearDeathMaterial;
  
  private Rigidbody ownRigidbody;
  
  private float currentHealth;
  private float actualSpeed;
  private float damagePerSecondReceiving = 0.0f;
  
  private short damageState = 0;
  
  private List<StatusEffect> activeStatusEffects;
  
  void Start() {
    ownRigidbody = GetComponent<Rigidbody>();
    activeStatusEffects = new List<StatusEffect>();
    
    currentHealth = maxHealth;
    actualSpeed = speed;
  }
  
  void Update() {
    RemoveHealth(damagePerSecondReceiving * Time.deltaTime);
    
    ownRigidbody.velocity = transform.forward * actualSpeed;
  }
  
  public void RemoveHealth(float damage) {
    currentHealth -= damage;
    
    if (currentHealth <= 0.0f) {
      Destroy(gameObject);
    }
    
    if ((currentHealth / maxHealth) <= 0.6f && damageState <= 0) {
      GetComponent<Renderer>().material = damagedMaterial;
      
      damageState = 1;
    } else if ((currentHealth / maxHealth) <= 0.3f && damageState <= 1) {
      GetComponent<Renderer>().material = nearDeathMaterial;
      
      damageState = 2;
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
  
  private void RefreshStatusEffects() {
    float maxSlowPercentage = 0.0f;
    float totalDamagePerSecond = 0.0f;
    
    foreach (StatusEffect statusEffect in activeStatusEffects) {
      if (statusEffect.slowPercentage > maxSlowPercentage) {
        maxSlowPercentage = statusEffect.slowPercentage;
      }
      
      totalDamagePerSecond += statusEffect.damagePerSecond;
    }
    
    actualSpeed = speed * ((100.0f - maxSlowPercentage) / 100.0f);
    damagePerSecondReceiving = totalDamagePerSecond;
  }
  
  public int GetPriority() {
    return priority;
  }
}
