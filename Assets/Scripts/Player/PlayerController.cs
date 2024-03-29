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

    [Header("Animation")]
    public Animator opossumAnimator;
    private Vector3 lastPosition;

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
    
    private bool isClimbing {
        get {
            return activeLadder != null && ladderCooldownTimer <= 0.0f;
        }
    }

    private float climbSpeed = 0.0f;
    
    private Ladder activeLadder = null;
    
    private float ladderCooldown = 0.5f;
    private float ladderCooldownTimer = 0.0f;

    void Update() {
        if (isClimbing) {
            ExecuteLadderMove();
        } else {
            RotateTowardsViewDirection();
            ExecuteMove();
            
            if (ladderCooldownTimer > 0.0f) {
                ladderCooldownTimer -= Time.deltaTime;
            }
        }

        SetAnimatorParams();
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
    
    private void ExecuteLadderMove() {
        if (Input.GetButtonDown("Jump")) {
            DisengageLadder();
            verticalSpeed = Vector3.up * jumpHeight;
            return;
        }
        
        float verticalClimb = Input.GetAxisRaw("Vertical") * (short) activeLadder.verticalClimbDirection;
        float horizontalClimb = Input.GetAxisRaw("Horizontal") * (short) activeLadder.horizontalClimbDirection;
        
        float climbDirection = Mathf.Clamp(verticalClimb + horizontalClimb, -1.0f, 1.0f);

        climbSpeed = climbDirection;
        
        if (climbDirection < 0.0f && controller.isGrounded) {
            DisengageLadder();
            return;
        }
        
        controller.Move(Vector3.up * Time.deltaTime * climbDirection * activeLadder.climbSpeed);
    }
    
    public void EngageLadder(Ladder ladder) {
        activeLadder = ladder;
        transform.rotation = Quaternion.Euler(0f, ladder.playerFaceDirection, 0f);
    }
    
    public void DisengageLadder() {
        activeLadder = null;
        ladderCooldownTimer = ladderCooldown;
    }

    private void SetAnimatorParams() {
        opossumAnimator.SetBool("isGrounded", controller.isGrounded);

        if (IsSliding) {
            opossumAnimator.SetFloat("moveSpeed", 0.0f);
        } else {
            float playerVelocity = ((transform.position - lastPosition) / Time.deltaTime).magnitude;
            opossumAnimator.SetFloat("moveSpeed", playerVelocity / moveSpeed);
        }

        opossumAnimator.SetBool("isClimbing", isClimbing);
        opossumAnimator.SetFloat("climbSpeed", climbSpeed);

        lastPosition = transform.position;
    }
}
