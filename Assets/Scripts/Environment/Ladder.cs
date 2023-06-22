using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public GameObject ladderTop;
    public GameObject ladderBottom;
    
    public float climbSpeed = 0.03f;
    
    public Vector3 GetBottomPosition() {
        return ladderBottom.transform.position;
    }
    
    public Vector3 GetTopPosition() {
        return ladderTop.transform.position;
    }
}
