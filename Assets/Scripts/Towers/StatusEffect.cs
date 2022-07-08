using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Effect", menuName = "Towers/Status Effect", order = 2)]
public class StatusEffect : ScriptableObject { 
  public float slowPercentage;
  
  public float damagePerSecond;
  
  public float duration;
}
