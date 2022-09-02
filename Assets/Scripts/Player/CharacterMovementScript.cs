using UnityEngine;
using UnityEngine.AI;

public class CharacterMovementScript : MonoBehaviour {
    public NavMeshAgent playerNavMeshAgent;

    public Camera playerCamera;

    void Update() {
        //if the left mouse button is clicked
        if (Input.GetMouseButton(0)) {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
                playerNavMeshAgent.SetDestination(raycastHit.point);
            }
        }
    }
}
