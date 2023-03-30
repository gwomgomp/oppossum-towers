using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour {
    public GameObject buildingMenu;
    public GameObject adjustMenu;

    [Header("Gameplay Settings")]
    public float maxDistanceToBuild = 0f;

    [Header("UI Settings")]
    public GameObject towerButtonPrefab;
    public float horizontalSpace;
    public float verticalSpace;

    public int maxButtonsPerRow;

    private GameObject openMenu;
    private BuildingSpot openBuildingSpot;

    private InteractableManager interactableManager;

    void Awake() {
        ManagerProvider.Instance.RegisterManager(this);
    }

    void Start() {
        var inputManager = ManagerProvider.Instance.GetManager<InputManager>();
        inputManager.RegisterInput(InputManager.InputType.Interact, Interact, 50);
        interactableManager = ManagerProvider.Instance.GetManager<InteractableManager>();
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

        if (openBuildingSpot != null && !IsSpotInRange(openBuildingSpot)) {
            CloseMenu();
        }
    }

    private bool Interact() {
        if (GetClosestBuildingSpot(out BuildingSpot closestBuildingSpot)) {
            if (openBuildingSpot == closestBuildingSpot) {
                CloseMenu();
            } else {
                DisplayMenu(closestBuildingSpot);
            }
            return true;
        } else {
            CloseMenu();
            return false;
        }
    }

    private bool GetClosestBuildingSpot(out BuildingSpot closestBuildingSpot) {
        return interactableManager.GetClosestInteractable(out closestBuildingSpot);
    }

    private bool IsSpotInRange(BuildingSpot buildingSpot) {
        return CalculateDistanceToSpot(buildingSpot) <= maxDistanceToBuild;
    }

    private float CalculateDistanceToSpot(BuildingSpot buildingSpot) {
        return Vector3.Distance(PlayerFinder.Instance.Player.transform.position, buildingSpot.transform.position);
    }

    private void DisplayMenu(BuildingSpot buildingSpot) {
        openBuildingSpot = buildingSpot;
        if (buildingSpot.Tower == null) {
            openMenu = buildingMenu;
            PopulateBuildingMenu();
        } else {
            openMenu = adjustMenu;
            PopulateAdjustMenu(buildingSpot);
        }
        openMenu.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(buildingSpot.transform.position);
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
            if (openMenu.Equals(buildingMenu)) {
                DepopulateBuildingMenu();
            }
            openMenu = null;
        }
    }

    private void PopulateBuildingMenu() {
        var buttonSize = towerButtonPrefab.GetComponent<RectTransform>().rect.size;
        var buttonWidth = buttonSize.x;
        var buttonHeight = buttonSize.y;

        var leftStart = CalculateLeftEndOfButtons(buttonWidth);

        var createdButtons = 0;
        var finishedRows = 0;
        var towerTypes = Resources.LoadAll<TowerBaseType>("Tower Types");
        foreach (TowerBaseType towerType in towerTypes) {
            var button = Instantiate(towerButtonPrefab, buildingMenu.transform, false);

            var horizontalPosition = leftStart - createdButtons * (buttonWidth + horizontalSpace);
            button.transform.Translate(Vector3.left * horizontalPosition);
            var verticalPosition = finishedRows * (buttonHeight + verticalSpace);
            button.transform.Translate(Vector3.down * verticalPosition);

            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.SetText(towerType.displayName);

            var buttonComponent = button.GetComponent<Button>();
            buttonComponent.interactable = openBuildingSpot.HasEnoughResourcesFor(towerType);
            buttonComponent.onClick.AddListener(() => Build(towerType));

            createdButtons++;
            if (createdButtons >= maxButtonsPerRow) {
                createdButtons = 0;
                finishedRows++;
            }
        }
    }

    private void PopulateAdjustMenu(BuildingSpot buildingSpot) {
        var buttonSize = towerButtonPrefab.GetComponent<RectTransform>().rect.size;
        var buttonWidth = buttonSize.x;
        var buttonHeight = buttonSize.y;

        var leftStart = CalculateLeftEndOfButtons(buttonWidth);

        var createdButtons = 0;
        var finishedRows = 1;
        var towerTypes = buildingSpot.Tower.towerType.upgrades;
        foreach (TowerUpgradeType towerType in towerTypes) {
            var button = Instantiate(towerButtonPrefab, adjustMenu.transform, false);

            var horizontalPosition = leftStart - createdButtons * (buttonWidth + horizontalSpace);
            button.transform.Translate(Vector3.left * horizontalPosition);
            var verticalPosition = finishedRows * (buttonHeight + verticalSpace);
            button.transform.Translate(Vector3.down * verticalPosition);

            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.SetText(towerType.displayName);

            var buttonComponent = button.GetComponent<Button>();
            buttonComponent.interactable = openBuildingSpot.HasEnoughResourcesFor(towerType);
            buttonComponent.onClick.AddListener(() => Build(towerType));

            createdButtons++;
            if (createdButtons >= maxButtonsPerRow) {
                createdButtons = 0;
                finishedRows++;
            }
        }
    }

    private float CalculateLeftEndOfButtons(float buttonWidth) {
        if (maxButtonsPerRow % 2 == 0) {
            return maxButtonsPerRow / 2 * (buttonWidth + horizontalSpace) - (buttonWidth / 2 + horizontalSpace / 2);
        } else {
            return Mathf.Floor(maxButtonsPerRow / 2) * (buttonWidth + horizontalSpace);
        }
    }

    private void DepopulateBuildingMenu() {
        foreach (Transform button in buildingMenu.transform) {
            Destroy(button.gameObject);
        }
    }
}
