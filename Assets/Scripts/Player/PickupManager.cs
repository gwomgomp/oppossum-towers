using UnityEngine;

// WIP, expand to allow holding multiple / different objects
public class PickupManager : MonoBehaviour {
    private Loot carrying;

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
        if (InteractableManager.Instance.GetClosestInteractable(out Hoard hoard)) {
            hoard.DepositLoot();
            Destroy(carrying.gameObject);
            carrying = null;
        }
    }

    private void PickUp() {
        if (InteractableManager.Instance.GetClosestInteractable(out Loot loot)) {
            carrying = loot;
            loot.AttachToTransform(PlayerFinder.Instance.Player.transform);
        }
    }
}
