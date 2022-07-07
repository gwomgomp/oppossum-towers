using UnityEngine;

public class Resource : MonoBehaviour
{
    private bool _initialized = false;
    private ResourceType _type = null;
    private float _weight = 0;
    private ResourceSpawnLocation _position = null;

    public void Initialize(ResourceType type, ResourceSpawnLocation startingPosition) {
        if (!_initialized) {
            _type = type;
            _weight = type.Weight;
            _position = startingPosition;
            transform.rotation = Quaternion.LookRotation(_position.transform.position);
            _initialized = true;
        } else {
            Debug.LogError("Do not try to initialize enemy twice");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
