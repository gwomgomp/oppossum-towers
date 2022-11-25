using UnityEngine;

public class Resource : MonoBehaviour {
    private bool initialized = false;
    private ResourceType type = null;
    private float weight = 0;
    private ResourceSpawnLocation resourceSpawnLocation = null;


    public void Initialize(ResourceType type, ResourceSpawnLocation startingPosition) {
        if (!initialized) {
            this.type = type;
            weight = type.Weight;
            resourceSpawnLocation = startingPosition;
            transform.rotation = Quaternion.LookRotation(resourceSpawnLocation.transform.position);
            initialized = true;
        } else {
            Debug.LogError("Do not try to initialize resource twice");
        }
    }
}
