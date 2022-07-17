using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour {
    public GameObject buildingMenu;

    private BuildingSpot openBuildingSpot;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f)) {
                if (hit.transform != null && hit.transform.gameObject.TryGetComponent(out BuildingSpot buildingSpot)) {
                    DisplayBuildingOptions(buildingSpot);
                }
            }
        }
    }

    private void DisplayBuildingOptions(BuildingSpot buildingSpot) {
        openBuildingSpot = buildingSpot;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(buildingSpot.transform.position);
        buildingMenu.SetActive(true);
        buildingMenu.transform.position = screenPosition;
    }

    public void Build(string type) {
        Debug.Log($"Building {type} on {openBuildingSpot.Name}");
        CloseBuildingOptions();
    }

    private void CloseBuildingOptions() {
        openBuildingSpot = null;
        buildingMenu.SetActive(false);
    }
}
