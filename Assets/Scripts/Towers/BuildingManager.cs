using UnityEngine;

public class BuildingManager : MonoBehaviour {
    public GameObject buildingMenu;
    public GameObject adjustMenu;

    private GameObject openMenu;
    private BuildingSpot openBuildingSpot;

    void Update() {
        if (
            Input.GetMouseButtonDown(0)
            && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500f)
            && hit.transform != null
            && hit.transform.gameObject.TryGetComponent(out BuildingSpot buildingSpot)
        ) {
            DisplayMenu(buildingSpot);
        }

        if (Input.GetMouseButtonDown(1) && openMenu != null) {
            CloseMenu();
        }
    }

    private void DisplayMenu(BuildingSpot buildingSpot) {
        openBuildingSpot = buildingSpot;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(buildingSpot.transform.position);
        if (buildingSpot.Tower == null) {
            openMenu = buildingMenu;
        } else {
            openMenu = adjustMenu;
        }
        openMenu.SetActive(true);
        openMenu.transform.position = screenPosition;
    }

    public void Build(TowerType type) {
        openBuildingSpot.Build(type);
        CloseMenu();
    }

    public void Upgrade() {
        Debug.Log($"Upgrading {openBuildingSpot.Name}");
        CloseMenu();
    }

    public void Delete() {
        openBuildingSpot.Clear();
        CloseMenu();
    }

    private void CloseMenu() {
        openBuildingSpot = null;
        openMenu.SetActive(false);
        openMenu = null;
    }
}
