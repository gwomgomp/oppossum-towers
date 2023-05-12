using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float minFov = 25;
    public float defaultFov = 55;
    public float maxFov = 90;
    public float zoomSpeed = 0.1f;

    private CinemachineVirtualCamera cinemachine;
    private float targetFov;

    void Start() {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        targetFov = defaultFov;
    }

    void Update() {
        HandleZoom();
    }

    private void HandleZoom() {
        var newFov = targetFov + Input.mouseScrollDelta.y * -1;
        targetFov = Mathf.Clamp(newFov, minFov, maxFov);
        cinemachine.m_Lens.FieldOfView = Mathf.MoveTowards(cinemachine.m_Lens.FieldOfView, targetFov, zoomSpeed);
    }
}
