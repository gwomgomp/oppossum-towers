using UnityEngine;

public class PickupManager : MonoBehaviour {
    private Cargo carrying;

    private InteractableManager interactableManager;

    void Start() {
        interactableManager = ManagerProvider.Instance.GetManager<InteractableManager>();
        var inputManager = ManagerProvider.Instance.GetManager<InputManager>();
        inputManager.RegisterInput(InputManager.InputType.Interact, Interact, 0);
    }

    private bool Interact() {
        if (carrying == null) {
            return PickUp();
        } else {
            return Drop();
        }
    }

    private bool Drop() {
        if (interactableManager.GetClosestInteractable(out Storage storage)) {
            if (storage.Store(carrying)) {
                Destroy(carrying.GetGameObject());
                carrying = null;
                return true;
            }
        }
        return false;
    }

    private bool PickUp() {
        if (interactableManager.GetClosestInteractable(out Cargo cargo)) {
            carrying = cargo;
            cargo.AttachToTransform(PlayerFinder.Instance.Player.transform);
            return true;
        }
        return false;
    }
}
