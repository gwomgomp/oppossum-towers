using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
    private void Start() {
        TryGetComponent(out SphereCollider collider);
        collider.radius = InteractableManager.Instance.MaxDistanceToInteract;
        PlayerIgnoreCollisionHelper.IgnorePlayerCollision(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (InteractableManager.Instance.IsRelevantEntry(other.gameObject)) {
            InteractableManager.Instance.StepIntoRange(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (InteractableManager.Instance.IsRelevantEntry(other.gameObject)) {
            InteractableManager.Instance.StepOutOffRange(this);
        }
    }
}
