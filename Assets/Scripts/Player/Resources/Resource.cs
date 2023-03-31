using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class Resource : MonoBehaviour, Cargo {
    private static float DroppingRange = 5;
    private static float FloatSpeed = 2;
    private static float ReturnDelay = 5;

    private bool initialized = false;
    private ResourceType type = null;
    private ResourceSpawnLocation resourceSpawnLocation = null;
    private ResourceSpawner resourceSpawner = null;

    private TweenerCore<Vector3, Vector3, VectorOptions> returnTween = null;

    public void Initialize(ResourceType type, ResourceSpawnLocation startingPosition, ResourceSpawner resourceSpawner) {
        if (!initialized) {
            this.type = type;
            resourceSpawnLocation = startingPosition;
            this.resourceSpawner = resourceSpawner;
            initialized = true;
            startingPosition.PlaceResource(this);
        } else {
            Debug.LogError("Do not try to initialize resource twice");
        }
    }

    public void AttachToTransform(Transform attachmentPoint) {
        if (returnTween != null) {
            returnTween.Kill();
            returnTween = null;
        }
        if (resourceSpawnLocation != null) {
            resourceSpawnLocation.TakeResource();
            resourceSpawnLocation = null;
        }
        PickupHelper.Attach(this, attachmentPoint);
    }

    public void DetachFromTransform() {
        PickupHelper.Detach(this);
         if (!IsHoardInRange()) {
            MoveToSpawner();
        }
    }

    private bool IsHoardInRange() {
        Hoard[] hoards = FindObjectsOfType<Hoard>();
        foreach (var hoard in hoards) {
            if (Vector3.Distance(transform.position, hoard.transform.position) < DroppingRange) {
                return true;
            }
        }
        return false;
    }

    private void MoveToSpawner() {
        returnTween = transform.DOMove(resourceSpawner.transform.position, FloatSpeed)
            .SetEase(Ease.InExpo)
            .SetSpeedBased(true)
            .SetDelay(ReturnDelay)
            .OnComplete(ReturnToSpawner);
    }

    private void ReturnToSpawner() {
        resourceSpawner.ReturnResource();
        Destroy(gameObject);
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public ResourceType GetResourceType() {
        return type;
    }
}
