using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Temporary helper class for tower targeting implementation, to be replaced with actual enemy class later
public class EnemyStats : MonoBehaviour
{
  public float health;
  // Arbitrary enemy priority to test different targeting methods
  public int priority;
  
  public Material defaultMaterial;
  public Material targetedMaterial;
  
  public void RemoveHealth(float damage)
  {
    health -= damage;
    
    if (health <= 0.0f)
    {
      Destroy(gameObject);
    }
  }
  
  public int GetPriority()
  {
    return priority;
  }
  
  // These two methods are purely for visualization purposes (i.e. serve no functional purpose)
  public void BeTargeted()
  {
    GetComponent<Renderer>().material = targetedMaterial;
  }
  
  public void BeUntargeted()
  {
    GetComponent<Renderer>().material = defaultMaterial;
  }
}
