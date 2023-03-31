using UnityEngine;

public class Resource : MonoBehaviour, Cargo {
    private bool initialized = false;
    private ResourceType type = null;
    private ResourceSpawnLocation resourceSpawnLocation = null;
    private ResourceSpawner resourceSpawner = null;

    public void Initialize(ResourceType type, ResourceSpawnLocation startingPosition, ResourceSpawner resourceSpawner) {
        if (!initialized) {
            this.type = type;
            resourceSpawnLocation = startingPosition;
            resourceSpawner = resourceSpawner;;
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
        if (IsSpawnerInRange()) {
            resourceSpawner.ReturnResource();
            Destroy(gameObject);

        } else if (!IsHoardInRange) {
            MoveToSpawner();
        }
    }

    private bool IsHoardInRange() {

    }

    private bool IsSpawnerInRange() {

    }

    private void MoveToSpawner() {

    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public ResourceType GetResourceType() {
        return type;
    }
}
