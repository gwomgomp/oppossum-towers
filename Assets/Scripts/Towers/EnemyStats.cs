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
  private List<TimedStatusEffect> timedStatusEffects;
  
  void Start() {
    ownRigidbody = GetComponent<Rigidbody>();
    activeStatusEffects = new List<StatusEffect>();
    timedStatusEffects = new List<TimedStatusEffect>();
    
    currentHealth = maxHealth;
    actualSpeed = speed;
  }
  
  void Update() {
    UpdateTimedStatusEffects();
    
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
    
    actualSpeed = speed * ((100.0f - maxSlowPercentage) / 100.0f);
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
    return priority;
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
