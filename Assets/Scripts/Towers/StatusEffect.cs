using UnityEngine;

[CreateAssetMenu(fileName = "Status Effect", menuName = "Towers/Status Effect", order = 2)]
public class StatusEffect : ScriptableObject {
    public float maxStacks;
    public float slowPercentage;
    public float weakenPercentage;
    public float damagePerSecond;
    public float duration;

}
