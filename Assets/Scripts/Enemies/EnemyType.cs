using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Type", menuName = "Enemies/Enemy Type", order = 1)]
public class EnemyType : ScriptableObject {
    [field: SerializeField]
    public float MaxHealth {get; private set;}
    [field: SerializeField]
    public float Speed {get; private set;}
    [field: SerializeField]
    public int Priority {get; private set;}
    [field: SerializeField]
    public Material TargetMaterial {get; private set;}
    [field: SerializeField]
    public Material DefaultMaterial {get; private set;}
}
