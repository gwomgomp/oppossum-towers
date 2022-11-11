using UnityEngine;

public class Loot : MonoBehaviour {
    void Start() {
        PlayerIgnoreCollisionHelper.IgnorePlayerCollision(gameObject);
    }

    internal void AttachToTransform(Transform attachmentPoint) {
        Interactable interactable = GetComponentInChildren<Interactable>();
        interactable.gameObject.SetActive(false);
        InteractableManager.Instance.StepOutOffRange(interactable);
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders) {
            collider.enabled = false;
        }
        transform.SetParent(attachmentPoint, true);
        transform.position = attachmentPoint.position;
        transform.Translate(Vector3.up * 2, Space.World);
    }

    internal void DetachFromTransform() {
        Interactable interactable = GetComponentInChildren<Interactable>(true);
        interactable.gameObject.SetActive(true);
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders) {
            collider.enabled = true;
        }
        transform.position = transform.parent.transform.position;
        transform.SetParent(transform.parent.parent, true);
    }
}
