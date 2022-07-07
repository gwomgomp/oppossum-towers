using UnityEngine;

public class ResourceSpawnLocation : MonoBehaviour
{
    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, .5f);
    }
}
