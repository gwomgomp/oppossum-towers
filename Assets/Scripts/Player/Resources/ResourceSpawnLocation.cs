using UnityEngine;

public class ResourceSpawnLocation : MonoBehaviour
{

    public bool IsResourcePlaced {
        get {
            return _isResourcePlaced;
        }
    }

    private bool _isResourcePlaced = false;

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
        _isResourcePlaced = false;
    }
    private void HandleResourcePlaced() {

        _isResourcePlaced = true;
    }
}
