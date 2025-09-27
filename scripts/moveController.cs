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
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody playerRB;
    private Animator playerAnimator;
    private Vector3 playerMoveDirection;
    private bool isWalking;

    private GroundCheck groundCheck;                        /// calling - groundCheck Raycast script
    private JumpController jumpController;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        groundCheck = GetComponent<GroundCheck>();          /// calling - GroundCheck Raycast script
        jumpController = GetComponent<JumpController>();    /// calling - JumpController


    }

    void Update()
    {
        /// --- Controller input ---
        AxisInput();
        if (groundCheck.IsGrounded == true)
        {
            Debug.Log("debug - IsGrounded: " + groundCheck.IsGrounded);               // debug                                                                          // Delegate jump handling
            jumpController.HandleJumpInput();                                         /// include jump animation
        }
        if (!groundCheck.IsGrounded)
        {
            Debug.Log("debug - IsGrounded: " + groundCheck.IsGrounded);               // debug
        }


    }

    /// Physics based time update
    void FixedUpdate()
    {
        /// --- move the player if detection of movement on the directional vecter mag of the playerMoveDirection
        if (playerMoveDirection.sqrMagnitude > 0.01f)   // playerMoveDirection.sqrMagnitude > 0.01f
        {
            isWalking = true;   
            MoveAndRotate();
            Debug.Log("debug - isWalking: " + isWalking);                            // debug

        }

        ///                                             TO DO !!
        
        /// If collision fall over , then wait x amount of time to geet up. - falling over animation ?

        /// --- Prevent tipping over ---
        /// This negats the physics 'ragdoll' effects. 
        /// Must figure to add it only upon collision of layers/targets - then negate constraints
        /// wait x amount of time then 'restand' the character to walk again (get up animation)
        //playerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;    

        else
        {
            isWalking = false;
            Debug.Log("debug - isWalking: " +isWalking);                             // debug
            Debug.Log("debug - isGrounded: " +groundCheck.IsGrounded);               // debug
        }


        // --- Animate WALKING 
        AnimateWalking();
    }



    /// --- Functions ---
    private void AxisInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(h, 0f, v).normalized;
    }

    /// -- Player Move direction + rotation
    private void MoveAndRotate()
    {
        /// --- Move player ---
        Vector3 velocity = playerMoveDirection * moveSpeed;
        playerRB.MovePosition(playerRB.position + velocity * Time.fixedDeltaTime);
        /// --- Rotate player to face movment direction ---
        Quaternion targetRotation = Quaternion.LookRotation(playerMoveDirection, Vector3.up);
        playerRB.MoveRotation(Quaternion.Slerp(playerRB.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }

 
    ///  --- Animation Functions --- 
    private void AnimateWalking()
    {
        playerAnimator.SetBool("isWalking", playerMoveDirection.sqrMagnitude > 0.01f);
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
