using UnityEngine;

[CreateAssetMenu(fileName = "Resource Type", menuName = "Resources/Resource Type", order = 1)]
public class ResourceType : ScriptableObject {
    [field: SerializeField]
    public float Weight { get; private set; }

    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
