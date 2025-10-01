using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PhysicsCharacterController : MonoBehaviour
{
    // Movement and character variables
    public float moveSpeed = 5f;    
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // -- private references
    private Rigidbody rb;
    private bool jumpInputPressed = false; // Flag to track jump input
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.freezeRotation = true; // Prevents the capsule from toppling over
        }

        // --- Important Check ---
        // Ask if GroundCheck object is not assigned
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck Transform is not assigned. Please assign it in the Inspector.");
        }
    }

    // Update is called once per frame (good for input)
    void Update()
    {
        // Check for jump input using the Space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputPressed = true;    
        }
    }

    // FixedUpdate is called at a fixed interval (good for physics)
    void FixedUpdate()
    {
        // --- Ground Check ---
    
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        //if (isGrounded == true) { Debug.Log("Touching layer: " + groundLayer); } //else { Debug.Log("Not Touching Layer" + groundLayer); }

        // --- Handle Input ---
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        // Get player input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        // Calculate movement direction
        Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical);

        // Apply movement force only if there is inpiut
        if (moveDirection.magnitude > 0)
        {
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);  // The velocity is kept cleaner by using a ForceMode.Force
        }
        else // Apply drag if no input is detected
        {
            //rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.95f, rb.linearVelocity.y, rb.linearVelocity.z * 0.95f);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.95f, rb.linearVelocity.y, rb.linearVelocity.z * 0.95f);
        }
    }

    void HandleJump()
    {
        // Handle jump logic in FixedUpdate, based on the flag set in Update
        if (jumpInputPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpInputPressed = false; // Reset the flag after performing the jump
        }
        else
        {
            jumpInputPressed = false; // Reset the flag if not grounded, preventing a queued jump mid-air
        }
    }

    /*
    //// Ground check function
    //bool IsGrounded()
    //{
    //    float capsuleHeight = GetComponent<CapsuleCollider>().height;
    //    float capsuleRadius = GetComponent<CapsuleCollider>().radius;
    //    Vector3 capsuleCenter = transform.position + Vector3.up * capsuleHeight / 2f;

    //    // Create a small sphere check at the bottom of the capsule
    //    return Physics.CheckCapsule(capsuleCenter, capsuleCenter + Vector3.down * (capsuleHeight / 2f + 0.1f), capsuleRadius, LayerMask.GetMask("ground"));
    //}
    */

    // --- Visualization for debugging ---
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
