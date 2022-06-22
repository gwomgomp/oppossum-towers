using UnityEngine;

public class ResourceLocation : MonoBehaviour
{
    public void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, .5f);
    }
}
