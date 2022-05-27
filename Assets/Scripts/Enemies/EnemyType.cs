using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Type", menuName = "Enemies/Enemy Type", order = 1)]
public class EnemyType : ScriptableObject {
    [field: SerializeField]
    public float MaxHealth {get; private set;}
    [field: SerializeField]
    public float Speed {get; private set;}
}
