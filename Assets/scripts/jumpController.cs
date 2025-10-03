using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GroundCheck))]
public class JumpController : MonoBehaviour
{
    public float jumpForce = 5f;

    private Rigidbody playerRB;                           /// script - moveController
    private GroundCheck groundCheck;
    private Animator animator;
    private bool jumpRequested;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
        groundCheck = GetComponent<GroundCheck>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        //if(groundCheck.IsGrounded == true)
        //{
        //    // Catch input instantly (frame-based)
        //    if (Input.GetKeyDown(KeyCode.Space) && groundCheck.IsGrounded)
        //    {
        //        jumpRequested = true;
        //        animator.SetTrigger("jump");   // fire animation immediately
        //    }
        //}


        // Keep animator updated with grounded state
        //animator.SetBool("isGrounded", groundCheck.IsGrounded);
    }

    /// FixedUpdate
    /*
void FixedUpdate()
    {
        // Catch input instantly
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck.IsGrounded)
        {
            jumpRequested = true;
        }
    }
    */

    public void HandleJump()
    {
        // Called in controllerMove.cs Update()
        if (jumpRequested)
        {
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // Fire jump animation
            animator.SetTrigger("jump");
            jumpRequested = false;
        }
    // Keep animator updated with grounded state
    animator.SetBool("isGrounded", groundCheck.IsGrounded);
    }



    public void jumpHandle()
    {
        if (groundCheck.IsGrounded == true)
        {
            // Catch input instantly (frame-based)
            if (Input.GetKeyDown(KeyCode.Space) && groundCheck.IsGrounded)
            {
                jumpRequested = true;
                animator.SetTrigger("jump");   // fire animation immediately
            }
        }
    }
}
