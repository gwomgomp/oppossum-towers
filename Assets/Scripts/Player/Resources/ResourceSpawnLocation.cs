using UnityEngine;

public class ResourceSpawnLocation : MonoBehaviour {

    public bool IsFree {
        get {
            return isFree;
        }
    }

    private bool isFree = true;

    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, .5f);
    }

    private void OnTriggerExit(Collider collider) {
        var resource = collider.GetComponent<Resource>();
        if (resource != null) HandleResourceTaken();
    }

    private void OnTriggerEnter(Collider collider) {
        var resource = collider.GetComponent<Resource>();
        if (resource != null) HandleResourcePlaced();
    }

    private void HandleResourceTaken() {
        var spawner = transform.parent.gameObject.GetComponent<ResourceSpawner>();
        // TODO: Add security that player cant just move in/out with object in hand
        spawner.ResourceTaken();
        isFree = true;
    }

    private void HandleResourcePlaced() {
        isFree = false;
    }
}
