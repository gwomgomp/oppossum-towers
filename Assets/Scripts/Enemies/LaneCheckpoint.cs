using UnityEngine;

public class LaneCheckpoint : MonoBehaviour
{
    [field: SerializeField]
    public LaneCheckpoint NextCheckpoint {get; set;}

    [field: SerializeField]
    public bool IsLootDropOff {get; private set;}

    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, .5f);
    }
}
