using UnityEngine;

public class ResourceSpawnLocation : MonoBehaviour, Placeable {

    private Resource currentResource;

    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, .5f);
    }

    public void PlaceResource(Resource resource) {
        currentResource = resource;
    }

    public Resource TakeResource() {
        Resource resource = currentResource;
        currentResource = null;
        return resource;
    }

    public bool IsFree() {
        return currentResource == null;
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}
