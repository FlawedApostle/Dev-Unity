using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// PlayerController.cs
/// Rigidbody-based movement using the Unity Input System (WASD / arrow keys).
/// File name MUST be PlayerController.cs and class name PlayerController.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float acceleration = 10f;

    [Header("Jump / Gravity Settings")]
    public float jumpForce = 5f; // placeholder for jump logic

    // Speed Settings
    Vector3 desiredVelocity;
    Vector3 newVelocity;

    // Cached Rigidbody
    private Rigidbody rb;
    private Vector3 currentVelocity;

    // Input System
    private Movement inputActions;   // Replace with your generated class name
    private Vector2 moveInput;
    private Vector3 inputDir;
    private bool jumpPressed;
    private bool sprintPressed;
    private bool crouchPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // Prevent ragdoll spinning

        // Instantiate Input Actions
        inputActions = new Movement();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        // Read input from Input System
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        ControllerJump();
        ConrollerSprint();
        ConrollerCrouch();
    }

    private void FixedUpdate()
    {
        // Calculate desired velocity based on input
        desiredVelocity = inputDir * moveSpeed;

        AccelerationSmoothLerp();

        // Preserve Y velocity (gravity/jumps handled elsewhere)
        newVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);
        rb.linearVelocity = newVelocity;

        // TODO: Add jump logic here, using jumpPressed
        // TODO: Add sprint/crouch modifiers if desired
    }

    private void AccelerationSmoothLerp()
    {
        // Smooth acceleration
        if (acceleration > 0f)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, Mathf.Clamp01(acceleration * Time.fixedDeltaTime));
        }
        else
        {
            currentVelocity = desiredVelocity;
        }
    }

    private void ControllerJump()
    {
        jumpPressed = inputActions.Player.Jump.triggered;
        if (jumpPressed == true) { Debug.Log("Jump Pressed !"); }
    }
    private void ConrollerSprint()
    {
        sprintPressed = inputActions.Player.Sprint.ReadValue<float>() > 0;
        if (sprintPressed == true) { Debug.Log("sprint Pressed !"); }
    }
    private void ConrollerCrouch()
    {
        crouchPressed = inputActions.Player.Crouch.ReadValue<float>() > 0;
        if (crouchPressed == true) { Debug.Log("crouch Pressed !"); }
    }
}
