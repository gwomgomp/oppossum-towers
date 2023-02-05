using UnityEngine;

public class Loot : MonoBehaviour, Cargo {
    void Start() {
        PlayerIgnoreCollisionHelper.IgnorePlayerCollision(gameObject);
    }

    public void AttachToTransform(Transform attachmentPoint) {
        PickupHelper.Attach(this, attachmentPoint);
    }

    public void DetachFromTransform() {
        PickupHelper.Detach(this);
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}
