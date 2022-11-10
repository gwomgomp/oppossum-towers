using UnityEngine;

public class LaneCheckpoint : MonoBehaviour, Placeable
{
    [field: SerializeField]
    public LaneCheckpoint NextCheckpoint {get; set;}

    [field: SerializeField]
    public bool IsLootDropOff {get; private set;}

    private void Start() {
        PlayerIgnoreCollisionHelper.IgnorePlayerCollision(gameObject);
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, .5f);
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}
