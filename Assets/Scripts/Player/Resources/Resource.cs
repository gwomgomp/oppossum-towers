using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private bool _initialized = false;
    private ResourceType type = null;
    private float _weight = 0;
    private LaneCheckpoint currentTarget = null;

    public void Initialize(ResourceType type, LaneCheckpoint startingCheckpoint) {
        if (!_initialized) {
            this.type = type;
            _weight = type.Weight;
            currentTarget = startingCheckpoint;
            transform.rotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
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
