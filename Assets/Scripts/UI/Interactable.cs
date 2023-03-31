using UnityEngine;

public class Interactable : MonoBehaviour {
    [SerializeField]
    private float distanceModifier = 1;

    private InteractableManager interactableManager;

    void Start() {
        interactableManager = ManagerProvider.Instance.GetManager<InteractableManager>();
        TryGetComponent(out SphereCollider collider);
        collider.radius = ManagerProvider.Instance.GetManager<InteractableManager>().MaxDistanceToInteract * distanceModifier;
    }

    private void OnTriggerEnter(Collider other) {
        if (interactableManager.IsRelevantEntry(other.gameObject)) {
            interactableManager.StepIntoRange(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (interactableManager.IsRelevantEntry(other.gameObject)) {
            interactableManager.StepOutOffRange(this);
        }
    }

    void OnDestroy() {
        interactableManager.StepOutOffRange(this);
    }
}
