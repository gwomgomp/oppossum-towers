using UnityEngine;

public class ResourceSpawnLocation : MonoBehaviour
{
    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, .5f);
    }

    private void HandleResourceTaken() {

        var spawner = transform.parent.gameObject.GetComponent<ResourceSpawner>();
        // TODO: Add security that player cant just move in/out with object in hand
        spawner.ResourceTaken();
    }

    private void OnTriggerExit(Collider collider) {

        var resource = collider.GetComponent<Resource>();
        if (resource != null) HandleResourceTaken();
    }
}
