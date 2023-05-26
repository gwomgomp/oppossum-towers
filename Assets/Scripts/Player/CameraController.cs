using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject thirdPersonCamera;
    public GameObject tacticalCamera;

    public float minFov = 25;
    public float defaultFov = 55;
    public float maxFov = 90;
    public float zoomSpeed = 0.1f;

    private CinemachineVirtualCamera thirdPersonCineMachine;
    private CinemachineVirtualCamera tacticalCineMachine;
    private float targetFov;

    void Start() {
        var inputManager = ManagerProvider.Instance.GetManager<InputManager>();
        inputManager.RegisterInput(InputManager.InputType.Camera, ToggleTacticalCamera, 0);
        thirdPersonCineMachine = thirdPersonCamera.GetComponent<CinemachineVirtualCamera>();
        tacticalCineMachine = tacticalCamera.GetComponent<CinemachineVirtualCamera>();
        targetFov = defaultFov;
    }

    void Update() {
        HandleZoom();
    }

    private void HandleZoom() {
        var newFov = targetFov + Input.mouseScrollDelta.y * -1;
        targetFov = Mathf.Clamp(newFov, minFov, maxFov);
        thirdPersonCineMachine.m_Lens.FieldOfView = Mathf.MoveTowards(thirdPersonCineMachine.m_Lens.FieldOfView, targetFov, zoomSpeed);
    }

    private bool ToggleTacticalCamera() {
        tacticalCineMachine.enabled = !tacticalCineMachine.enabled;
        return true;
    }
}
