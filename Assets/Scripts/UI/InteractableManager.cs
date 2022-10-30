using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableManager : MonoBehaviour {
    [field: SerializeField]
    public float MaxDistanceToInteract { get; private set; }

    private HashSet<Interactable> interactablesInRange = new();

    public static InteractableManager Instance { get; private set; }

    private GameObject closestInteractable;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Update() {
        GameObject newClosestInteractable = interactablesInRange
            .OrderBy(i => CalculateDistance(i))
            .Select(i => i.gameObject)
            .FirstOrDefault();

        if (newClosestInteractable != null && (closestInteractable == null || !closestInteractable.Equals(newClosestInteractable))) {
            if (closestInteractable != null) {
                closestInteractable.TryGetComponent(out MeshRenderer oldRenderer);
                oldRenderer.enabled = false;
            }
            newClosestInteractable.TryGetComponent(out MeshRenderer newRenderer);
            newRenderer.enabled = true;
            closestInteractable = newClosestInteractable;
        } else if (newClosestInteractable == null && closestInteractable != null) {
            closestInteractable.TryGetComponent(out MeshRenderer renderer);
            renderer.enabled = false;
            closestInteractable = null;
        }
    }

    public bool GetClosestInteractable<T>(out T closest) {
        if (closestInteractable == null) {
            closest = default;
            return false;
        }

        closest = closestInteractable.GetComponentInParent<T>();
        if (closest == null) {
            return false;
        } else {
            return true;
        }
    }

    private float CalculateDistance(Interactable interactable) {
        return Vector3.Distance(interactable.transform.position, PlayerFinder.Instance.Player.transform.position);
    }

    public bool IsRelevantEntry(GameObject entry) {
        return entry.Equals(PlayerFinder.Instance.Player);
    }

    public void StepIntoRange(Interactable interactable) {
        interactablesInRange.Add(interactable);
    }

    public void StepOutOffRange(Interactable interactable) {
        interactablesInRange.Remove(interactable);
    }
}
