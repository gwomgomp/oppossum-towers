using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public enum VerticalClimbDirection {
        None,
        Up,
        Down
    }
    
    public enum HorizontalClimbDirection {
        None,
        Right,
        Left
    }
    
    public float climbSpeed = 10.0f;
    
    public VerticalClimbDirection verticalClimbDirection;
    public HorizontalClimbDirection horizontalClimbDirection;
    
    public float playerFaceDirection;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out PlayerController playerController)) {
            playerController.EngageLadder(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.TryGetComponent(out PlayerController playerController)) {
            playerController.DisengageLadder();
        }
    }
}
