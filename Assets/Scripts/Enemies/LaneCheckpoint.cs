using UnityEngine;

public class LaneCheckpoint : MonoBehaviour
{
    [field: SerializeField]
    public LaneCheckpoint NextCheckpoint {get; private set;}
}
