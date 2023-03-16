using UnityEngine;

public class PlayerController : MonoBehaviour {
    public CharacterController controller;

    [Header("Horizontal Movement Settings")]
    public float moveSpeed = 6f;
    public float sprintModifier = 1.5f;
    public float walkModifier = 0.5f;

    [Header("Vertical Movement Settings")]
    public float jumpHeight = 3f;
    public float maxFallSpeed = 15f;
    public float gravityMultiplier = 1f;

    [Header("Slide Settings")]
    public float slideSpeed = 6f;

    [Header("Turn Settings")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private bool IsSliding {
        get {
            if (controller.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeCheckHit, 2f)) {
                var slopeCheckHitNormal = slopeCheckHit.normal;
                SlideMovement = Vector3.Cross(Vector3.Cross(slopeCheckHitNormal, Vector3.down), slopeCheckHitNormal);
                return Vector3.Angle(Vector3.up, slopeCheckHitNormal) > controller.slopeLimit;
            } else {
                return false;
            }
        }
    }

    private Vector3 gravity = new(0, -9.81f, 0);
    private Vector3 GravityMovement {
        get {
            return !controller.isGrounded ? gravity * gravityMultiplier : Vector3.zero;
        }
    }

    private Vector3 slideDirection = Vector3.zero;
    private Vector3 SlideMovement {
        get {
            return IsSliding ? slideDirection.normalized * slideSpeed : Vector3.zero;
        }
        set {
            slideDirection = value;
        }
    }

    private Vector3 InputMovement {
        get {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            float sprintSpeed = controller.isGrounded && Input.GetButton("Sprint") ? sprintModifier : 1f;
            float walkSpeed = controller.isGrounded && Input.GetButton("Walk") ? walkModifier : 1f;
            return Quaternion.AngleAxis(-45, Vector3.up) * new Vector3(horizontal, 0f, vertical).normalized * moveSpeed * sprintSpeed * walkSpeed;
        }
    }

    private Vector3 verticalSpeed = Vector3.zero;
    private Vector3 VerticalMovement {
        get {
            if (controller.isGrounded && !IsSliding && Input.GetButtonDown("Jump")) {
                verticalSpeed = Vector3.up * jumpHeight;
            }
            verticalSpeed += GravityMovement * Time.deltaTime;
            if (verticalSpeed.y > 0) {
                return verticalSpeed;
            } else {
                return Vector3.ClampMagnitude(verticalSpeed, maxFallSpeed);
            }
        }
    }

    void Update() {
        RotateTowardsViewDirection();
        ExecuteMove();
    }

    private void ExecuteMove() {
        Vector3 moveDirection = InputMovement + SlideMovement + VerticalMovement;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void RotateTowardsViewDirection() {
        if (InputMovement.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(InputMovement.x, InputMovement.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
}
