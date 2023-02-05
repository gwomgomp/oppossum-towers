using UnityEngine;

public interface Cargo {
    public void AttachToTransform(Transform transform);
    public void DetachFromTransform();
    public GameObject GetGameObject();
}
