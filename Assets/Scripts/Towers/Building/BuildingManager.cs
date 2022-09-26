using UnityEngine;
using System.Linq;

public class BuildingManager : MonoBehaviour {
    public GameObject buildingMenu;
    public GameObject adjustMenu;

    public float maxDistanceToBuild = 0f;

    private GameObject openMenu;
    private BuildingSpot openBuildingSpot;

    private GameObject player;
    private BuildingSpot[] availableBuildingSpots;

    void Start() {
        MonoBehaviour playerScript = FindObjectOfType<ThirdPersonMovement>();
        if (playerScript == null) {
            playerScript = FindObjectOfType<CharacterMovementScript>();
        }
        if (playerScript != null) {
            player = playerScript.gameObject;
        }
        availableBuildingSpots = FindObjectsOfType<BuildingSpot>();
    }

    void Update() {
        if (
            Input.GetMouseButtonDown(0)
            && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500f)
            && hit.transform != null
            && hit.transform.gameObject.TryGetComponent(out BuildingSpot selectedBuildingSpot)
            && IsSpotInRange(selectedBuildingSpot)
        ) {
            DisplayMenu(selectedBuildingSpot);
        }

        if (Input.GetMouseButtonDown(1)) {
            CloseMenu();
        }

        if (Input.GetButtonDown("Interact")) {
            if (GetClosestBuildingSpot(out BuildingSpot closestBuildingSpot)) {
                if (openBuildingSpot == closestBuildingSpot) {
                    CloseMenu();
                } else {
                    DisplayMenu(closestBuildingSpot);
                }
            } else {
                CloseMenu();
            }
        }

        if (openBuildingSpot != null && !IsSpotInRange(openBuildingSpot)) {
            CloseMenu();
        }
    }

    // show building range on player
    private void OnDrawGizmosSelected() {
        if (player != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.transform.position, maxDistanceToBuild);
        }
    }

    private bool GetClosestBuildingSpot(out BuildingSpot closestBuildingSpot) {
        closestBuildingSpot = availableBuildingSpots
            .Select(buildingSpot => (buildingSpot, distance: CalculateDistanceToSpot(buildingSpot)))
            .Where(tuple => tuple.distance <= maxDistanceToBuild)
            .OrderBy(tuple => tuple.distance)
            .Select(tuple => tuple.buildingSpot)
            .FirstOrDefault();

        if (closestBuildingSpot == null) {
            return false;
        } else {
            return true;
        }
    }

    private bool IsSpotInRange(BuildingSpot buildingSpot) {
        return CalculateDistanceToSpot(buildingSpot) <= maxDistanceToBuild;
    }

    private float CalculateDistanceToSpot(BuildingSpot buildingSpot) {
        return player == null ? -1 : Vector3.Distance(player.transform.position, buildingSpot.transform.position);
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
        // Does nothing for now
        Debug.Log($"Upgrading {openBuildingSpot.Name}");
        CloseMenu();
    }

    public void Delete() {
        openBuildingSpot.Clear();
        CloseMenu();
    }

    private void CloseMenu() {
        if (openMenu != null) {
            openBuildingSpot = null;
            openMenu.SetActive(false);
            openMenu = null;
        }
    }
}
