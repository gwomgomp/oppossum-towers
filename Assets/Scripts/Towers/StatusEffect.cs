using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Effect", menuName = "Towers/Status Effect", order = 2)]
public class StatusEffect : ScriptableObject {
    public DamageType damageType;
    public float maxStacks;
    public float slowPercentage;
    public float weakenPercentage;
    public float damagePerSecond;
    public float duration;
    [SerializeField()]
    public List<TriggerEffect> triggeringEffects;
}

[Serializable]
public class TriggerEffect {
    public DamageType damageType;
    public StatusEffect effect;

    public bool isTriggered;
}
