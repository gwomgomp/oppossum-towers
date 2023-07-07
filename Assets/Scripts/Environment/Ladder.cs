using UnityEngine;

public class Ladder : MonoBehaviour
{
    public enum VerticalClimbDirection : short {
        None = 0,
        Up = 1,
        Down = -1
    }
    
    public enum HorizontalClimbDirection : short {
        None = 0,
        Right = 1,
        Left = -1
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
