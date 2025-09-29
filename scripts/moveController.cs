using System;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundCheck))]
[RequireComponent(typeof(JumpController))]
// Player Move Script


public class MovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float mouseSensitivity = 0.5f;
    public float moveSpeed = 1f;
    public float rotationSpeed = 1f;
    private float v, h;
    [Header("Camera Settings")]
    public FirstPersonCamera cam; // assign in Inspector


    private Rigidbody playerRB;
    private Animator playerAnimator;
    private Vector3 playerMoveDirection;
    private bool isWalking;

    private GroundCheck groundCheck;                        /// calling - groundCheck Raycast script
    private JumpController jumpController;

    // NEW: flag to sync Update input with FixedUpdate physics
    private bool jumpRequested;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();          /// animation
        groundCheck = GetComponent<GroundCheck>();          /// calling - GroundCheck Raycast script
        jumpController = GetComponent<JumpController>();    /// calling - JumpController


    }
    // np physics based movement
    void Update()
    {
        // Capture jump input instantly
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck.IsGrounded)
        {
            jumpRequested = true;
            playerAnimator.SetTrigger("jump");   // fire animation immediately
        }
    }
        ///                                             TO DO !!
        /// If collision fall over , then wait x amount of time to geet up. - falling over animation ?
        /// --- Prevent tipping over ---
        /// This negats the physics 'ragdoll' effects. 
        /// Must figure to add it only upon collision of layers/targets - then negate constraints
        /// wait x amount of time then 'restand' the character to walk again (get up animation)
    /// Physics based time update
    void FixedUpdate()
    {
        AxisInput();
        //debug_groundCheck();
        /// jump
        jumpController.HandleJump();
        //jumpController.jumpHandle();

        /// Movement
        // --- Movement relative to camera ---
        Vector3 camForward = Vector3.ProjectOnPlane(cam.Forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cam.Right, Vector3.up).normalized;
        Vector3 moveDir = (camForward * v + camRight * h).normalized;
        if (moveDir.sqrMagnitude > 0.01f) // moveDir        playerMoveDirection
        {
            isWalking = true;
            playerMoveDirection = new Vector3(moveDir.x, 0f, moveDir.z); // flatten Y
            MoveAndRotate();
        }
        else
        {
            isWalking = false;
        }

        AnimateWalking();
    }



    /// ---------------------------------------------------------------------- Functions -----------------------------------------------------------
    private void AxisInput()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(h, 0f, v).normalized;
    }

    /// -- Player Move direction + rotation
    private void MoveAndRotate()
    {
        /// --- Move player ---
        Vector3 velocity = playerMoveDirection * moveSpeed;
        playerRB.MovePosition(playerRB.position + velocity * Time.fixedDeltaTime);
        // --- Smoothly rotate player to face movement direction ---
        if (playerMoveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerMoveDirection, Vector3.up);
            playerRB.MoveRotation(
                Quaternion.Slerp(playerRB.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        /// --- Rotate player to face movment direction ---
        /*
        Quaternion targetRotation = Quaternion.LookRotation(playerMoveDirection, Vector3.up);
        playerRB.MoveRotation(Quaternion.Slerp(playerRB.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        //playerRB.MoveRotation(Quaternion.Slerp(playerRB.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        */
    }




    ///  --- Animation Functions --- 
    private void AnimateWalking()
    {
        isWalking = true;
        playerAnimator.SetBool("isWalking", playerMoveDirection.sqrMagnitude > 0.01f);
    }


    /// --- Debug functions
    private void debug_groundCheck()
    {
        if (groundCheck.IsGrounded == true)
        {
            Debug.Log("debug - IsGrounded: " + groundCheck.IsGrounded);               // debug
        }
        if (!groundCheck.IsGrounded)
        {
            Debug.Log("debug - IsGrounded: " + groundCheck.IsGrounded);               // debug

        }
    }



    private bool IsUpright()
    {
        float uprightDot = Vector3.Dot(transform.up, Vector3.up);
        return uprightDot > 0.9f;
    }

    private void KeepUpright()
    {
        Quaternion current = transform.rotation;
        Quaternion target = Quaternion.Euler(0, current.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(current, target, 10f * Time.deltaTime);
    }
}
