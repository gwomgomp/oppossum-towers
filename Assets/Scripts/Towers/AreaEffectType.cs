using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Area Effect Type", menuName = "Towers/Area Effect Type", order = 3)]
public class AreaEffectType : ScriptableObject {
    public float duration;
    public bool snapToGround;

    public StatusEffect statusEffect;
}
