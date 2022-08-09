using UnityEngine;

public class BuildingSpot : MonoBehaviour {
    [field: SerializeField]
    public string Name { get; private set; }

    public Tower Tower { get; private set; }

    public void Build(TowerType type) {
        var enemyPrefab = Resources.Load<GameObject>("Prefabs/Tower");
        var enemyGameObject = Instantiate(enemyPrefab, transform.position, transform.rotation);
        Tower = enemyGameObject.GetComponent<Tower>();
        Tower.type = type;
    }

    public void Clear() {
        Destroy(Tower.gameObject);
        Tower = null;
    }
}
