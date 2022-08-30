using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float turnSmoothtime = 0.1f;
    float turnSmoothVelocity;

    bool isGrounded;
    Vector3 moveDirection;

    //Sliding parameters
    private float slopeSpeed = 1f;
    private Vector3 hitPointNormal;
    private bool IsSliding{
        get{
            if(controller.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f)){
                hitPointNormal = slopeHit.normal;
                Debug.Log(Vector3.Angle(hitPointNormal, Vector3.up) > controller.slopeLimit);
                return Vector3.Angle(hitPointNormal, Vector3.up) > controller.slopeLimit;
            }else{
                return false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && moveDirection.y < 0 && !IsSliding){
            moveDirection = Vector3.zero;
            moveDirection.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f){
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothtime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //this is breaking stuff
            moveDirection = direction * speed;

            //controller.Move(direction * speed * Time.deltaTime);
        }

        if(Input.GetButtonDown("Jump") && isGrounded){
            moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if(!isGrounded){
            Debug.Log("in the air");
            moveDirection.y += gravity * Time.deltaTime;
        }

        //Slippery Slope
        if(IsSliding){
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        }
        
        controller.Move(moveDirection * Time.deltaTime);
    }
}
