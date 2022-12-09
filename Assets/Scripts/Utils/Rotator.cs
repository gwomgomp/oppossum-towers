using UnityEngine;

public class Rotator : MonoBehaviour {
    [SerializeField]
    private Vector3 rotationSpeed;
    [SerializeField]
    private Vector3 randomizationFactors;

    public Vector3 randomization;

    void Start() {
        Vector3 random = Random.insideUnitSphere - new Vector3(0.5f, 0.5f, 0.5f);
        randomization = Vector3.Scale(random, randomizationFactors);
    }

    void Update() {
        Vector3 toRotate = rotationSpeed + Vector3.Scale(rotationSpeed, randomization);
        transform.Rotate(toRotate * Time.deltaTime, Space.World);
    }
}
