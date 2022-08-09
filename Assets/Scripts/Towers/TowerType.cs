using UnityEngine;

[CreateAssetMenu(fileName = "Tower Type", menuName = "Towers/Tower Type", order = 1)]
public class TowerType : ScriptableObject {
    [field: SerializeField]
    public float DamagePerShot {get; private set;}
    [field: SerializeField]
    public float ShotCooldown {get; private set;}
    [field: SerializeField]
    public float Range {get; private set;}
}
