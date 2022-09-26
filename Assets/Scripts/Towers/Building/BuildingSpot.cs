using UnityEngine;

public class BuildingSpot : MonoBehaviour {
    [field: SerializeField]
    public string Name { get; private set; }

    public Tower Tower { get; private set; }

    public void Build(TowerType type) {
        var towerPrefab = Resources.Load<GameObject>("Prefabs/Tower");
        var towerGameObject = Instantiate(towerPrefab, transform.position, transform.rotation);
        Tower = towerGameObject.GetComponent<Tower>();
        Tower.towerType = type;
    }

    public void Clear() {
        Destroy(Tower.gameObject);
        Tower = null;
    }
}
