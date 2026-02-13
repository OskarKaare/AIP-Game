using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Jobs;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] private float mouseSpeed = 0.5f;
    private Camera playerCamera;
    private float swimSpeed = 75f;
    private float walkSpeed = 5f;
    private float verticalSpeed = 30f;
    private float maxSpeed = 15f;
    private float swimDrift = 0.1f;

    private Vector3 currentVelocity = Vector3.zero;
    private float decelerationTime = 0.5f;
    private float cameraXRotation = 0f;
    [SerializeField] private float minLookAngle = -90f;
    [SerializeField] private float maxLookAngle = 90f;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementStyle();
        Look();
        //Vertical();
    }

    //void Vertical()
    //{
    //    // Stop if player is using Move action
    //    if (InputSystem.actions["Move"].ReadValue<Vector2>().magnitude > 0.1f)
    //    {
    //        currentVerticalVelocity = 0f;
    //        return;
    //    }

    //    float targetVerticalVelocity = 0f;
        
    //    // Check if Jump is pressed (ascend)
    //    if (InputSystem.actions["Jump"].IsPressed())
    //    {
    //        targetVerticalVelocity = verticalSpeed;
    //    }
        
    //    // Check if Crouch is pressed (descend)
    //    if (InputSystem.actions["Crouch"].IsPressed())
    //    {
    //        targetVerticalVelocity = -verticalSpeed;
    //    }
        
    //    // Lerp vertical velocity
    //    float lerpSpeed = Mathf.Abs(targetVerticalVelocity) > 0.1f ? swimDrift : 1f / decelerationTime;
    //    currentVerticalVelocity = Mathf.Lerp(currentVerticalVelocity, targetVerticalVelocity, Time.deltaTime * lerpSpeed);
        
    //    Vector3 verticalMove = transform.up * currentVerticalVelocity * Time.deltaTime;
    //    controller.Move(verticalMove);
    //}
    
    void MovementStyle()
        {
        if (transform.position.y > -2.5f)
        {
            Walk();
        }
        else 
        {
            Swim();
        }
    }

    void Walk()
    {    
        Vector2 input = InputSystem.actions["Move"].ReadValue<Vector2>();
        Vector3 move = (transform.right * input.x + transform.forward * input.y) *walkSpeed ;
            verticalSpeed = -9f;
            move.y = verticalSpeed;
        animator.SetFloat("Forward", input.y);
        animator.SetFloat("Strafe", input.x);
        controller.Move(move * Time.deltaTime);

        // set ani bool for walking
        if (InputSystem.actions["Move"].IsPressed() && transform.position.y > -2.5f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }



    }
    void Swim()
    {
        Vector2 input = InputSystem.actions["Move"].ReadValue<Vector2>();
        Vector3 targetVelocity = new Vector3(0,0,0);

        if (input.magnitude > 0.1f)
        {
            // Player is pressing movement keys - move in that direction
            targetVelocity = (playerCamera.transform.right * input.x + playerCamera.transform.forward * input.y) * swimSpeed;

       
        }
    
        else
        {
            targetVelocity = Vector3.zero;
        }

        // lerp between the current velo and the targeted
        float lerpSpeed = input.magnitude > 0.1f ? swimDrift : 1f / decelerationTime;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * lerpSpeed);
        // set max speed
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

        controller.Move(currentVelocity * Time.deltaTime);
        if(InputSystem.actions["Jump"].IsPressed())
        {
            controller.Move(Vector3.up * -verticalSpeed * Time.deltaTime);
        }
        else if(InputSystem.actions["Crouch"].IsPressed())
        {
            controller.Move(Vector3.down * -verticalSpeed * Time.deltaTime);
        }
      

        if (transform.position.y <= -2.5f)
        {
            animator.SetBool("inWater", true);
            if (InputSystem.actions["Move"].IsPressed() && transform.position.y <= -2.5f)
            {
                animator.SetBool("isSwimming", true);
            }
            else
            {
                animator.SetBool("isSwimming", false);
            }
        }
        else
        {
            animator.SetBool("inWater", false);

        }

        //Debug.Log(currentVelocity.magnitude);
    }
  

    void Look()
    {
        Vector2 mouseInput = InputSystem.actions["Look"].ReadValue<Vector2>();
        transform.Rotate(Vector3.up * mouseInput.x * mouseSpeed);

        cameraXRotation -= mouseInput.y * mouseSpeed;
        cameraXRotation = Mathf.Clamp(cameraXRotation, minLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraXRotation, 0f, 0f);
    }
}
