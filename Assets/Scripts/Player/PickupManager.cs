using UnityEngine;

public class PickupManager : MonoBehaviour {
    private Cargo carrying;

    void Update() {
        if (Input.GetButtonDown("Interact")) {
            if (carrying == null) {
                PickUp();
            } else {
                Drop();
            }
        }
    }

    private void Drop() {
        if (InteractableManager.Instance.GetClosestInteractable(out Storage storage)) {
            if (storage.Store(carrying)) {
                Destroy(carrying.GetGameObject());
                carrying = null;
            }
        }
    }

    private void PickUp() {
        if (InteractableManager.Instance.GetClosestInteractable(out Cargo cargo)) {
            carrying = cargo;
            cargo.AttachToTransform(PlayerFinder.Instance.Player.transform);
        }
    }
}
