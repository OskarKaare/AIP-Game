using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Jobs;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] private float mouseSpeed = 0.5f;
    private Camera playerCamera;
    private float verticalSpeed = 5;
    private float swimSpeed = 75;
    private float maxSpeed = 15;
    private float swimDrift = 0.1f;
    private Vector3 currentVelocity = Vector3.zero;
    private float currentVerticalVelocity = 0f;
    private float decelerationTime = 0.5f;
    private float cameraXRotation = 0f;
    [SerializeField] private float minLookAngle = -90f;
    [SerializeField] private float maxLookAngle = 90f;
    private bool isBoosting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Swim();
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
    
    void Swim()
    {
        Vector2 input = InputSystem.actions["Move"].ReadValue<Vector2>();
        Vector3 targetVelocity;
        
        if (input.magnitude > 0.1f)
        {
            // Player is pressing movement keys - move in that direction
            targetVelocity = (playerCamera.transform.right * input.x + playerCamera.transform.forward * input.y) * swimSpeed;
           
        }
        else
        {
            // No input - slow down to zero
            targetVelocity = Vector3.zero;
        }
        
        // Smoothly interpolate current velocity toward target velocity
        float lerpSpeed = input.magnitude > 0.1f ? swimDrift : 1f / decelerationTime;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * lerpSpeed);
        if(!isBoosting)
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        
        controller.Move(currentVelocity * Time.deltaTime);

        Debug.Log(currentVelocity.magnitude);
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
