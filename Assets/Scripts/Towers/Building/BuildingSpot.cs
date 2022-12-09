using System.Collections.Generic;
using UnityEngine;

public class BuildingSpot : MonoBehaviour, Placeable, Storage {
    [field: SerializeField]
    public string Name { get; private set; }

    public Tower Tower { get; private set; }

    private List<Resource> depositedResources = new();

    private void Start() {
        PlayerIgnoreCollisionHelper.IgnorePlayerCollision(gameObject);
    }

    public void Build(TowerType type) {
        var towerPrefab = Resources.Load<GameObject>("Prefabs/Tower");
        var towerGameObject = Instantiate(towerPrefab, transform.position, transform.rotation);
        Tower = towerGameObject.RequireComponent<Tower>();
        Tower.towerType = type;
    }

    public void Clear() {
        Destroy(Tower.gameObject);
        Tower = null;
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public bool Store(Cargo cargo) {
        if (cargo is Resource resource) {
            depositedResources.Add(resource);
            return true;
        } else {
            return false;
        }
    }
}
