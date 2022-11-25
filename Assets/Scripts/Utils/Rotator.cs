using UnityEngine;

public class Rotator : MonoBehaviour {
    [SerializeField]
    private Vector3 rotationSpeed;

    void Update() {
        Vector3 toRotate = rotationSpeed * Time.deltaTime;
        transform.Rotate(toRotate, Space.World);
    }
}
