using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableManager : MonoBehaviour {
    [field: SerializeField]
    public float MaxDistanceToInteract { get; private set; }

    private HashSet<Interactable> interactablesInRange = new();

    public static InteractableManager Instance { get; private set; }

    private GameObject player;
    private GameObject closestInteractable;

    void Start() {
        MonoBehaviour playerScript = FindObjectOfType<ThirdPersonMovement>();
        if (playerScript == null) {
            playerScript = FindObjectOfType<CharacterMovementScript>();
        }
        if (playerScript != null) {
            player = playerScript.gameObject;
        }
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Update() {
        GameObject newClosestInteractable = interactablesInRange
            .OrderBy(i => Vector3.Distance(i.transform.position, player.transform.position))
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
        }
    }

    public bool GetClosestInteractable<T>(out T closest) {
        closest = interactablesInRange
            .OrderBy(i => Vector3.Distance(i.transform.position, player.transform.position))
            .Select(i => i.GetComponentInParent<T>())
            .Where(c => c != null)
            .FirstOrDefault();

        if (closest == null) {
            return false;
        } else {
            return true;
        }
    }

    public bool IsRelevantEntry(GameObject entry) {
        return entry.Equals(player);
    }

    public void StepIntoRange(Interactable interactable) {
        interactablesInRange.Add(interactable);
    }

    public void StepOutOffRange(Interactable interactable) {
        interactablesInRange.Remove(interactable);
    }
}
