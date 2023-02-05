using UnityEngine;

public class Resource : MonoBehaviour, Cargo {
    private bool initialized = false;
    private ResourceType type = null;
    private ResourceSpawnLocation resourceSpawnLocation = null;

    public void Initialize(ResourceType type, ResourceSpawnLocation startingPosition) {
        if (!initialized) {
            this.type = type;
            resourceSpawnLocation = startingPosition;
            initialized = true;
            startingPosition.PlaceResource(this);
        } else {
            Debug.LogError("Do not try to initialize resource twice");
        }
    }

    public void AttachToTransform(Transform attachmentPoint) {
        resourceSpawnLocation.TakeResource();
        resourceSpawnLocation = null;
        PickupHelper.Attach(this, attachmentPoint);
    }

    public void DetachFromTransform() {
        PickupHelper.Detach(this);
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}
