using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour {
    public CharacterController controller;

    public float speed = 6f;
    public float maxSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Vector3 moveDirection = Vector3.zero;

    //Sliding parameters
    public float slideSpeed = 10f;
    private Vector3 hitPointNormal;
    private bool isSliding = false;

    void Update() {
        Vector3 direction = CalculateDirection();
        Vector3 moveDirection = CalculateMoveDirection(direction);

        controller.Move(moveDirection * Time.deltaTime);

        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private Vector3 CalculateDirection() {
        if (isSliding) {
            return Vector3.Cross(Vector3.Cross(hitPointNormal, Vector3.down), hitPointNormal);
        } else {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            return new Vector3(horizontal, 0f, vertical).normalized;
        }
    }

    private Vector3 CalculateMoveDirection(Vector3 direction) {
        if (isSliding) {
            float angleFactor = Mathf.Clamp(Vector3.Dot(direction, Vector3.down) * 2, 0.1f, 1f);
            return direction * slideSpeed * angleFactor;
        } else {
            if (direction.magnitude >= 0.1f) {
                moveDirection += direction * speed;
                // fucky because we don't want to limit total speed, only movement speed with jumping excluded
                Vector2 clampedMoveDirection = Vector2.ClampMagnitude(new Vector2(moveDirection.x, moveDirection.z), maxSpeed);
                moveDirection.x = clampedMoveDirection.x;
                moveDirection.z = clampedMoveDirection.y;
            } else {
                // stop movement when direction is not pointing anywhere
                moveDirection.x = 0;
                moveDirection.z = 0;
            }

            if (controller.isGrounded) {
                if (Input.GetButtonDown("Jump")) {
                    moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            } else {
                moveDirection.y += gravity * Time.deltaTime;
            }

            return moveDirection;
        }
    }

    // this way works for angles to about 80 degrees and only if the player doesn't slide onto another surface that slides
    // you also can't jump when colliding with a wall since any collision is detected (filter through some wall mask?)
    // TODO refactor / find better way
    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit) {
            if (!isSliding) {
                hitPointNormal = hit.normal;
                isSliding = true;
            }
        } else {
            isSliding = false;
        }
    }
}
